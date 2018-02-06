using DynamicStepsLib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMgrModels;
using TaskMgrTypes;
using TaskMgrTypes.Constants;

namespace TaskMgrConsole
{
    public class StepExecution : IJob
    {
        public StepExecution()
        {
        }

        public Task Execute(IJobExecutionContext jonExecutionContext)
        {
            string connectionString = jonExecutionContext.JobDetail.JobDataMap[ConfigKey.ConnectionString].ToString();
            string dllWithPath = jonExecutionContext.JobDetail.JobDataMap[ConfigKey.DllWithPath].ToString();
            int queueStepId = int.Parse(jonExecutionContext.JobDetail.JobDataMap[NonCatogarized.QueueStepPK].ToString());

            DynamicStepsUtil util = new DynamicStepsUtil();

            var optionsBuilder = new DbContextOptionsBuilder<TaskMgrContext>();
            optionsBuilder.UseSqlServer(connectionString);
            DateTime curDateTime = DateTime.Now;

            using (var dbContext = new TaskMgrContext(optionsBuilder.Options))
            {
                var qstp = dbContext.QueueSteps.Include(qs => qs.Step).Include(r => r.Queue).First(r => r.QueueStepId == queueStepId);

                try
                {

                    string stepStatus = qstp.Status;

                    var lastQueueStep = dbContext.QueueSteps.Include(r => r.Step).Where(r => r.QueueId == qstp.QueueId && r.Seq < qstp.Seq).OrderByDescending(r => r.Seq).FirstOrDefault();

                    var task = (from q in dbContext.Queues
                                join s in dbContext.Schedules on q.ScheduleId equals s.ScheduleId
                                join t in dbContext.Tasks on s.TaskId equals t.TaskId
                                where q.QueueId == qstp.QueueId
                                select t).FirstOrDefault();

                    // update step status to in progress

                    if (!qstp.ExecutionStarted.HasValue)
                    {
                        qstp.ExecutionStarted = DateTime.Now;
                        qstp.Status = QueueStatus.InProgress;
                        dbContext.SaveChanges();
                    }

                    //// get returned values from prior step

                    //var lastQueueStep = dbContext.QueueSteps.Include(r => r.Step).Where(r => r.QueueId == qstp.QueueId && r.Seq < qstp.Seq).OrderByDescending(r => r.Seq).FirstOrDefault();
                    //if (lastQueueStep != null)
                    //{
                    //}

                    string priorStepReturnValues = "";
                    if (lastQueueStep != null)
                    {
                        priorStepReturnValues = lastQueueStep.ReturnValues;
                    }

                    if (task.EmailsOnStepStart && stepStatus == QueueStepStatus.Added)
                    {
                        // queue email
                        EmailExecution.AddEmailQueue(task.StartedEmails, "", "Task : " + task.Name + ". Step : " + qstp.Step.Name + " - Started", "");

                    }

                    InputValues inputValues = new InputValues { QueueStepId = qstp.QueueStepId, ScheduledStart = qstp.Queue.ScheduledStart, Values = priorStepReturnValues };
                    

                    string stepClass = qstp.Step.Class;
                    bool isIdled = false;
                    try
                    {
                        Console.WriteLine("Step Execution Started : " + stepClass);

                        OutputValues result = util.ExecuteStep(dllWithPath, stepClass, inputValues);
                        isIdled = result.IsIdled;

                        if (!isIdled)
                        {
                            qstp.PostExecutionDecision = result.PostExecutionDecision;
                        }
                        qstp.FailureInfo = JsonConvert.SerializeObject(result.FailureInfo);
                        qstp.ReturnValues = result.Values;

                        if (!isIdled)
                        {
                            Console.WriteLine("Step Execution Completed : " + stepClass);
                            // mark step completed regardless on completion status
                            qstp.Status = QueueStepStatus.Completed;
                            qstp.ExecutionCompleted = DateTime.Now;

                            if (task.EmailsOnStepComplete)
                            {
                                // queue email
                                EmailExecution.AddEmailQueue(task.CompletedEmails, "", "Task : " + task.Name + ". Step " + " : " + qstp.Step.Name + " Completed with Execution Status "
                                                    + qstp.PostExecutionDecision, "");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Step Idled : " + stepClass);
                        }

                        dbContext.SaveChanges();

                    }
                    catch (Exception ex)
                    {
                        Program.LogException(new ExceptionInfo { Message = "Error Executing Step (" + qstp.Step.Name + ") : " + ex.Message });
                    }
                }
                catch (Exception ex)
                {
                    Program.LogException(new ExceptionInfo { Message = "Error Before or After Step (" + qstp.Step.Name + ") Execution : " + ex.Message });
                }

                return Task.FromResult(0);
            }
        }
    }
}
