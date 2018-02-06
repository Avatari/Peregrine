using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Collections.Specialized;
using Quartz.Impl;
using TaskMgrTypes;
using TaskMgrTypes.Constants;

namespace TaskMgrConsole
{

    class Program
    {
        public static IScheduler Scheduler; // add this field
        public static IConfigurationRoot Configuration;
        static void Main(string[] args)
        {
            try
            {
                Console.Title = "Peregrine Task Manager Console. (Ctrl+X to exit)";

                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                Configuration = builder.Build();

                // read email password and set in EmailExecution class
                // this is simple logic does not use encryption
                // Strongly recommended to change this to use more secure method as per your need
                FileStream fileStream = new FileStream(Configuration[ConfigKey.TaskMgrInfoFile], FileMode.Open);
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    string pwd = reader.ReadLine();
                    EmailExecution.SetPassword(pwd);
                }

                var schedulerFactory = new StdSchedulerFactory();

                Scheduler = schedulerFactory.GetScheduler().Result;

                Scheduler.Start().Wait();

                StartHeartBeats().GetAwaiter().GetResult();

            }
            catch (Exception ex)
            {
                Program.LogException(new ExceptionInfo { Message = "Error Running Application : " + ex.Message });
            }

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey();

                if (key.Modifiers == ConsoleModifiers.Control && (key.KeyChar == 24))
                {
                    Console.WriteLine("Exiting..");
                    break;
                }
            }
        }


        public static async Task StartHeartBeats()
        {
            try
            {
                int ScheduleIntervalInSeconds = int.Parse(Configuration[ConfigKey.ScheduleIntervalInSeconds]); //job will run every minute
                int emailIntervalInSeconds = int.Parse(Configuration["EmailIntervalInSeconds"]); // email queue execution job interval


                JobKey jobKey = JobKey.Create("HeartBeatJob");

                string connectionString = Configuration.GetConnectionString(ConfigKey.ConnectionString);
                string dllWithPath = Configuration[ConfigKey.DllWithPath];

                IJobDetail job = JobBuilder.Create<HeartBeat>().UsingJobData(ConfigKey.ConnectionString, connectionString).UsingJobData(ConfigKey.DllWithPath, dllWithPath).WithIdentity(jobKey).Build();

                ITrigger heartBeatTrigger = TriggerBuilder.Create()
                    .WithIdentity("HeartBeatJobTrigger")
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(ScheduleIntervalInSeconds).RepeatForever())
                    .Build();

                // start email job
                JobKey emailJobKey = JobKey.Create("EmailJobTrigger");
                IJobDetail emailJob = JobBuilder.Create<EmailExecution>().WithIdentity(emailJobKey).Build();
                ITrigger emailJobTrigger = TriggerBuilder.Create()
                .WithIdentity("EmailJobTrigger")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(emailIntervalInSeconds).RepeatForever())
                .Build();
                await Scheduler.ScheduleJob(emailJob, emailJobTrigger);

                // start and wait heart beat job
                await Scheduler.ScheduleJob(job, heartBeatTrigger);
            }
            catch (Exception ex)
            {
                Program.LogException(new ExceptionInfo { Message = "Error starting Heart Beats : " + ex.Message });
            }
        }

        public static void LogException(ExceptionInfo exInfo)
        {
            Console.WriteLine(exInfo.ToString());
        }

    }
}
