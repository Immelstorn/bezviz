using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

using LinqToTwitter;

using Quartz;
using Quartz.Impl;

namespace Bezviz
{
    public class Program
    {
        public static void Main()
        {
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler();
            scheduler.Start();

            var job = JobBuilder.Create<TwitterJob>().Build();

            var trigger = TriggerBuilder.Create()
                            .WithSimpleSchedule(x => x.WithIntervalInHours(24).RepeatForever())
                            .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}
