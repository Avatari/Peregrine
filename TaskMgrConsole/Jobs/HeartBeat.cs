using Microsoft.EntityFrameworkCore;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMgrModels;
using TaskMgrTypes.Constants;
using TaskMgrTypes;

namespace TaskMgrConsole
{
    public class HeartBeat : IJob
    {
        public HeartBeat()
        {
        }

        public Task Execute(IJobExecutionContext jonExecutionContext)
        {
            try
            {
                string connectionString = jonExecutionContext.JobDetail.JobDataMap[ConfigKey.ConnectionString].ToString();
                string dllWithPath = jonExecutionContext.JobDetail.JobDataMap[ConfigKey.DllWithPath].ToString();

                var optionsBuilder = new DbContextOptionsBuilder<TaskMgrContext>();
                optionsBuilder.UseSqlServer(connectionString);
                DateTime curDateTime = DateTime.Now;

                using (var dbContext = new TaskMgrContext(optionsBuilder.Options))
                {
                    // update last heart beat in db
                    KeyValues lastHeartBeatRow = dbContext.KeyValues.FirstOrDefault(r => r.Key == KeyValuesTableKey.LastHeartBeat);
                    if (lastHeartBeatRow != null)
                    {
                        lastHeartBeatRow.Dtval = DateTime.Now;
                        dbContext.SaveChanges();
                    }

                    CheckRunTaskSteps.CheckRunQueueSteps(dbContext, curDateTime, connectionString, dllWithPath);
                }
                Console.WriteLine(DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                Program.LogException(new ExceptionInfo { Message = "Error while Heart Beat Execution : " + ex.Message });
            }

            return Task.FromResult(0);
        }

    }
}
