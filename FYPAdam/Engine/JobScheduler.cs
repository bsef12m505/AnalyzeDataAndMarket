using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Engine
{
    public class JobScheduler
    {
        public static void Start()
        {
            try
            {
                // Construct a scheduler factory
                ISchedulerFactory schedFact = new StdSchedulerFactory();

                // get a scheduler
                IScheduler sched = schedFact.GetScheduler();
                sched.Start();

                //Create a Job
                IJobDetail job = JobBuilder.Create<LoggingJob>()
                    .WithIdentity("myJob", "group1")
                    .Build();

                //create a job for sending mail to the user
                IJobDetail userJob = JobBuilder.Create<UserEmailJob>()
                    .WithIdentity("userJob", "group1")
                    .Build();

                //ITrigger trigger = TriggerBuilder.Create()
                //                .StartNow().WithDailyTimeIntervalSchedule
                //                  (s =>
                //                     s.WithIntervalInHours(1)
                //                    .OnEveryDay()
                //                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                //                  )
                //                .Build();

                ITrigger trigger = TriggerBuilder.Create()
                 .WithIdentity("trigger1", "group2")
                 .StartNow()
                 .WithSimpleSchedule(x => x
                     .WithIntervalInSeconds(180)
                     .RepeatForever())
                 .Build();

                //user email trigger
                ITrigger emailTrigger = TriggerBuilder.Create()
                .WithIdentity("trigger2", "group2")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(7200)
                    .RepeatForever())
                .Build();

                //Schedule a Job
                sched.ScheduleJob(userJob, emailTrigger);
                sched.ScheduleJob(job, trigger);

            }
            catch (ArgumentException e)
            {

            }
        }


    }
}