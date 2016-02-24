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
                     .WithIntervalInSeconds(864000)
                     .RepeatForever())
                 .Build();

                //Schedule a Job
                sched.ScheduleJob(job, trigger);
            }
            catch (ArgumentException e)
            {

            }
        }


    }
}