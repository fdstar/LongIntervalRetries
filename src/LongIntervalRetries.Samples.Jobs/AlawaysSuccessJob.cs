using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace LongIntervalRetries.Samples.Jobs
{
    public class AlawaysSuccessJob : SimpleJob
    {
        protected override string JobName => "AlawaysSuccessJob";
        public async override Task Execute(IJobExecutionContext context)
        {
            await base.Execute(context);
            Logger.Info("{0}_{1} is finished.", JobName,JobContextId);
        }
    }
}
