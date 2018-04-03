using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace LongIntervalRetries.Samples.Jobs
{
    public class MaybeFailJob : SimpleJob
    {
        const int errorSeed = 3;
        const int errorNumber = 0;
        protected override string JobName => "MaybeFailJob";
        public async override Task Execute(IJobExecutionContext context)
        {
            await base.Execute(context);
            if (errorSeed.Next() == errorNumber)
            {
                Logger.Error("{0}_{1} will thrown an Exception.", JobName, JobContextId);
                throw new Exception(JobName);
            }
            Logger.Info("Luckly {0}_{1} is finished without any Exception.", JobName, JobContextId);
        }
    }
}
