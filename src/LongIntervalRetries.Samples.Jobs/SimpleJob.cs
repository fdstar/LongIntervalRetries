using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LongIntervalRetries.Samples.Jobs
{
    public class SimpleJob : IJob
    {
        public static readonly Logger Logger = LogManager.GetLogger("JobLogger");
        protected virtual string JobName { get; } = "SimpleJob";
        protected int JobContextId { get; set; }
        const int maxSleepMilliseconds = 3000;
        public virtual Task Execute(IJobExecutionContext context)
        {
            var id = context.MergedJobDataMap.GetInt("Id");
            JobContextId = id;
            Logger.Info("{0}_{1} is running...", JobName, JobContextId);
            //var rd = maxSleepMilliseconds.Next();
            //if (rd > 0)
            //{
            //    Logger.Info("{0}_{1} will sleep {2} ms.", JobName, JobContextId, rd);
            //    Thread.Sleep(rd);
            //}
            return Task.FromResult(1);
        }
    }
}
