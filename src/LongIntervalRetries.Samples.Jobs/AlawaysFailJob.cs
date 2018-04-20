using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace LongIntervalRetries.Samples.Jobs
{
    //[PersistJobDataAfterExecution]
    public class AlawaysFailJob : SimpleJob
    {
        protected override string JobName => "AlawaysFailJob";
        public async override Task Execute(IJobExecutionContext context)
        {
            await base.Execute(context);
            context.MergedJobDataMap["Time"] = DateTime.Now;
            Logger.Error("{0} will thrown an Exception.", JobName);
            throw new Exception(JobName);
        }
    }
}
