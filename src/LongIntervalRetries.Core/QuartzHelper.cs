using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries.Core
{
    internal class QuartzHelper
    {
        public static ITrigger BuildTrigger(DateTimeOffset? startAt)
        {
            return TriggerBuilder.Create()
                .WithIdentity(Guid.NewGuid().ToString(), StdRetry.RetryGroupName)
                .StartAt(startAt ?? DateTimeOffset.UtcNow)
                .WithSimpleSchedule(x => x.WithRepeatCount(0))
                .Build();
        }
        public static IJobDetail BuildJob<T>(JobDataMap map) where T : IJob
        {
            return BuildJob(typeof(T), map);
        }
        public static IJobDetail BuildJob(Type type, JobDataMap map)
        {
            return JobBuilder.Create(type)
                .WithIdentity(Guid.NewGuid().ToString(), StdRetry.RetryGroupName)
                .SetJobData(map)
                .Build();
        }
    }
}
