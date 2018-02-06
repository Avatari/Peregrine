using TaskMgrTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskMgrModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using DynamicStepsLib;
using System.Threading.Tasks;
using Quartz;
using TaskMgrTypes.Constants;

namespace TaskMgrConsole
{
    public class CheckRunTaskSteps
    {
        public static void CheckRunQueueSteps(TaskMgrContext dbContext, DateTime heartBeatDT, string connectionString, string dllWithPath)
        {
            // Step1: Find/Queue tasks
            QueueTasks(dbContext, heartBeatDT);

            // Step2: Find/Queue steps
            QueueSteps(dbContext, heartBeatDT);

            // Step3: Find steps need to Execute and schedule immediate using Quartz
            RunSteps(dbContext, heartBeatDT, connectionString, dllWithPath);
        }

        private static void RunSteps(TaskMgrContext dbContext, DateTime heartBeatDT, string connectionString, string dllWithPath)
        {
            try
            {
                // find steps need to be execute
                // get values need to pass from prior step return values
                // when execution is complete save return values, status etc.
                List<QueueSteps> queueSteps = dbContext.QueueSteps.Where(r => r.Status != QueueStatus.Completed
                                                && r.Status != QueueStatus.InProgress).ToList();
                foreach (var qstp in queueSteps)
                {
                    string queueStepId = qstp.QueueStepId.ToString();
                    JobKey jobKey = JobKey.Create("StepExecution" + qstp.QueueStepId.ToString());

                    IJobDetail job = JobBuilder.Create<StepExecution>().UsingJobData(ConfigKey.ConnectionString, connectionString)
                                                                    .UsingJobData(ConfigKey.DllWithPath, dllWithPath)
                                                                    .UsingJobData(NonCatogarized.QueueStepPK, queueStepId).WithIdentity(jobKey).Build();

                    ITrigger heartBeatTrigger = TriggerBuilder.Create()
                        .WithIdentity("StepExecution" + qstp.QueueStepId.ToString() + "Trigger")
                        .StartNow()
                        .Build();

                    Program.Scheduler.ScheduleJob(job, heartBeatTrigger);
                }
            }
            catch (Exception ex)
            {
                Program.LogException(new ExceptionInfo { Message = "Error while setting Step Execution Triggers : " + ex.Message });
            }
        }

        private static void QueueTasks(TaskMgrContext dbContext, DateTime heartBeatDT)
        {
            try
            {
                // find all schedules need to be queue and queue them
                // criteria: must be disabled, not completed, then if nextqueue is empty it has never been queued, else nextqueue date must meet date/time criterias
                foreach (var sch in dbContext.Schedules.Where(r => !r.Disabled && !r.IsCompleted && heartBeatDT >= r.Start
                                                        && (!r.NextQueue.HasValue || heartBeatDT >= r.NextQueue.Value)).ToList())
                {
                    DateTime? queueDT;
                    if (sch.NextQueue.HasValue)
                    {
                        queueDT = sch.NextQueue.Value;
                    }
                    else
                    {
                        // get first time queuing datetime, then check if heartBeatDT is >= this value, then only queue otherwise skip
                        DateTime? firstQueueDT;
                        if (QualifyToStartSchedule(sch.Freq, sch.PeriodInterval, sch.Start, heartBeatDT, out firstQueueDT))
                        {
                            // first time queuing, get corrected queue date time returned from function
                            queueDT = firstQueueDT;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    AddQueueRec(dbContext, sch, queueDT);

                    // get next queue datetime
                    DateTime? nextQueueDT = GetNextQueueDTBasedOnLastQueueDT(sch.Freq, sch.PeriodInterval, queueDT.Value);

                    if (nextQueueDT.HasValue && (!sch.EndDt.HasValue || nextQueueDT.Value <= sch.EndDt.Value))
                    {
                        // if next queue date time has valid value 
                        sch.NextQueue = nextQueueDT;
                    }
                    else
                    {
                        // mark schedule fulfilled
                        sch.NextQueue = null;
                        sch.IsCompleted = true;
                    }
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Program.LogException(new ExceptionInfo { Message = "Error Queuing Tasks  : " + ex.Message });
            }

        }

        private static void QueueSteps(TaskMgrContext dbContext, DateTime heartBeatDT)
        {
            try
            {
                foreach (var que in dbContext.Queues.Include(q => q.Schedule).ThenInclude(sch => sch.Task).Where(q => q.Status != QueueStatus.Completed && !q.Cancelled))
                {
                    var lastQueueStp = dbContext.QueueSteps.Where(s => s.QueueId == que.QueueId).OrderByDescending(s => s.Seq).FirstOrDefault();
                    TaskSteps nextStp = null;
                    bool allStepsCompleted = false;

                    if (lastQueueStp != null)
                    {
                        // check status of last step is completed somehow (successfully / failed / any new status : basically need to move forward in flow)
                        if (lastQueueStp.Status != QueueStepStatus.Added && lastQueueStp.Status != QueueStatus.InProgress && lastQueueStp.Status != QueueStepStatus.Idled)
                        {
                            // based on status and goto seq of TaskStep decide next step
                            var lastTaskStep = dbContext.TaskSteps.Where(ts => ts.TaskId == que.Schedule.TaskId && ts.Seq == lastQueueStp.Seq).First();
                            int nextSeq = 0;

                            if (string.IsNullOrEmpty(lastTaskStep.PostExecutionDecision))
                            {
                                nextSeq = lastTaskStep.GotoSeq.HasValue ? lastTaskStep.GotoSeq.Value : 0;
                            }
                            else
                            {
                                if (lastTaskStep.PostExecutionDecision == lastQueueStp.PostExecutionDecision)
                                {
                                    nextSeq = lastTaskStep.GotoSeq.Value;
                                }
                            }

                            if (nextSeq <= 0)
                            {
                                nextStp = dbContext.TaskSteps.Where(ts => ts.TaskId == que.Schedule.TaskId && ts.Seq > lastQueueStp.Seq).OrderBy(ts => ts.Seq).FirstOrDefault();
                            }
                            else
                            {
                                nextStp = dbContext.TaskSteps.Where(ts => ts.TaskId == que.Schedule.TaskId && ts.Seq == nextSeq).OrderBy(ts => ts.Seq).FirstOrDefault();
                            }
                            allStepsCompleted = (nextStp == null);
                        }
                    }
                    else
                    {
                        // queue is not started, so add first task step in queue steps
                        nextStp = dbContext.TaskSteps.Where(ts => ts.TaskId == que.Schedule.TaskId).OrderBy(ts => ts.Seq).FirstOrDefault();

                        allStepsCompleted = (nextStp == null);      // No steps exist for Task which should not happen but ..
                    }

                    if (nextStp != null)
                    {
                        AddQueueStepRec(dbContext, que, nextStp);
                        que.Status = QueueStatus.InProgress;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        if (allStepsCompleted)
                        {
                            que.Status = QueueStatus.Completed;
                            que.Completed = DateTime.Now;
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.LogException(new ExceptionInfo { Message = "Error Queuing Steps  : " + ex.Message });
            }
        }

        private static bool QualifyToStartSchedule(string freq, int? freqIdent, DateTime startDT, DateTime heartBeatDT, out DateTime? firstQueueDT)
        {
            // function checks whether schedule is qulified to initiate first queue based on frequence/start/heartBeatDT
            firstQueueDT = null;

            DateTime? correctedStartDT = null;

            switch (freq)
            {
                case Frequency.OneTimeCode:
                case Frequency.RepeatDailyCode:
                case Frequency.RepeatAfterXMinutesCode:
                    firstQueueDT = startDT;
                    break;
                case Frequency.DayOfWeekCode:
                    correctedStartDT = DateTimeLib.WeekDay(startDT, freqIdent.Value);
                    break;
                case Frequency.DayOfMonthCode:
                    correctedStartDT = DateTimeLib.MonthDay(startDT, freqIdent.Value);
                    break;
                case Frequency.FirstDayOfMonthCode:
                    correctedStartDT = DateTimeLib.FirstDayOfMonth(startDT);
                    break;
                case Frequency.FirstBusinessDayOfMonthCode:
                    correctedStartDT = DateTimeLib.FirstBusinessDayOfMonth(startDT);
                    break;
                case Frequency.LastDayOfMonthCode:
                    correctedStartDT = DateTimeLib.FirstBusinessDayOfMonth(startDT);
                    break;
                case Frequency.LastBusinessDayOfMonthDescription:
                    correctedStartDT = DateTimeLib.LastBusinessDayOfMonth(startDT);
                    break;
            }

            if (!firstQueueDT.HasValue && correctedStartDT.HasValue)
            {
                if (heartBeatDT >= correctedStartDT)
                {
                    firstQueueDT = correctedStartDT;
                }
            }

            return firstQueueDT.HasValue;
        }

        private static DateTime? GetNextQueueDTBasedOnLastQueueDT(string freq, int? freqIdent, DateTime lastQueueDT)
        {
            DateTime? nextQueue = null;

             
            // based on scuedule 
            switch(freq)
            {
                case Frequency.RepeatDailyCode:
                    nextQueue = lastQueueDT.AddDays(1);
                    break;
                case Frequency.RepeatAfterXMinutesCode:
                    nextQueue = lastQueueDT.AddMinutes(freqIdent.Value);
                    break;
                case Frequency.DayOfWeekCode:
                    nextQueue = lastQueueDT.AddDays(7);
                    break;
                case Frequency.DayOfMonthCode:
                    nextQueue = DateTimeLib.NextMonthDay(lastQueueDT, freqIdent.Value);
                    break;
                case Frequency.FirstDayOfMonthCode:
                    nextQueue = DateTimeLib.FirstDayOfNextMonth(lastQueueDT);
                    break;
                case Frequency.FirstBusinessDayOfMonthCode:
                    nextQueue = DateTimeLib.FirstBusinessDayOfNextMonth(lastQueueDT);
                    break;
                case Frequency.LastDayOfMonthCode:
                    nextQueue = DateTimeLib.LastDayOfNextMonth(lastQueueDT);
                    break;
                case Frequency.LastBusinessDayOfMonthDescription:
                    nextQueue = DateTimeLib.LastBusinessDayOfNextMonth(lastQueueDT);
                    break;
            }

            return nextQueue;
        }

        private static void AddQueueRec(TaskMgrContext dbContext, Schedules sch, DateTime? nextQueueDT)
        {
            Queues que = new Queues();
            que.ScheduleId = sch.ScheduleId;
            que.ScheduledStart = nextQueueDT.Value;
            que.Status = QueueStatus.Added;

            dbContext.Queues.Add(que);
            dbContext.SaveChanges();
        }


        private static void AddQueueStepRec(TaskMgrContext dbContext, Queues que, TaskSteps taskStp)
        {
            QueueSteps qStp = new QueueSteps();

            qStp.QueueId = que.QueueId;
            qStp.Seq = taskStp.Seq;
            qStp.StepId = taskStp.StepId;
            
            qStp.Added = DateTime.Now;
            qStp.Status = QueueStepStatus.Added;

            dbContext.QueueSteps.Add(qStp);
        }
    }
}
