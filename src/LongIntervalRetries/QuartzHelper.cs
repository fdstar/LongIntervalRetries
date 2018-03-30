using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries
{
    internal class QuartzHelper
    {
        public static ITrigger BuildTrigger(DateTimeOffset? startAt, string name = null)
        {
            return TriggerBuilder.Create()
                .WithIdentity(name ?? Guid.NewGuid().ToString(), StdRetrySetting.RetryGroupName)
                .StartAt(startAt ?? DateTimeOffset.UtcNow)
                .WithSimpleSchedule(x => x.WithRepeatCount(0))
                .Build();
        }
        public static IJobDetail BuildJob<T>(JobDataMap map, string name = null) where T : IJob
        {
            return BuildJob(typeof(T), map, name);
        }
        public static IJobDetail BuildJob(Type type, JobDataMap map, string name = null)
        {
            return JobBuilder.Create(type)
                .WithIdentity(name ?? Guid.NewGuid().ToString(), StdRetrySetting.RetryGroupName)
                .SetJobData(map)
                .Build();
        }
    }
}
