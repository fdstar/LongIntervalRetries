using Quartz.Listener;
using System;
using System.Collections.Generic;
using System.Text;
using Quartz;
using System.Threading;
using System.Threading.Tasks;
using LongIntervalRetries.Rules;

namespace LongIntervalRetries
{
    internal class StdRetryJobListener : JobListenerSupport, IRetryJobListener
    {
        private IRetryRuleManager _ruleManager;
        public event RetryJobExecuted JobExecuted;
        public StdRetryJobListener(IRetryRuleManager ruleManager)
        {
            this._ruleManager = ruleManager;
        }
        public override string Name => "LongIntervalRetries.RetryJobListener";

        public override Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Trigger.Key.Group == StdRetrySetting.RetryGroupName)
            {
                this.Deal(context, jobException);
            }
            return base.JobWasExecuted(context, jobException, cancellationToken);
        }
        private async void Deal(IJobExecutionContext context, JobExecutionException jobException)
        {
            var jobMap = context.JobDetail.JobDataMap;
            var executedNumber = jobMap.GetIntValue(StdRetrySetting.ExecutedNumberContextKey) + 1;
            if (executedNumber < 1) executedNumber = 1;
            var removeKeies = new string[] { StdRetrySetting.ExecutedNumberContextKey, StdRetrySetting.RetryRuleNameContextKey, StdRetrySetting.RetryStoredInfoIdContextKey };
            var executedJobMap = new Dictionary<string, object>(jobMap);
            var jobType = context.JobDetail.JobType;
            object storeId = null;
            if (jobMap.ContainsKey(StdRetrySetting.RetryStoredInfoIdContextKey))
            {
                storeId = jobMap[StdRetrySetting.RetryStoredInfoIdContextKey];
            }
            foreach (var key in removeKeies)
            {
                executedJobMap.Remove(key);
            }
            var jobStatus = RetryJobStatus.Completed;
            var ruleName = jobMap.GetString(StdRetrySetting.RetryRuleNameContextKey);
            if (jobException != null)
            {
                jobStatus = RetryJobStatus.Canceled;
                var ts = this._ruleManager.GetRule(ruleName)?.GetNextFireSpan(executedNumber);
                if (ts >= TimeSpan.Zero)
                {
                    jobStatus = RetryJobStatus.Continue;
                    var trigger = QuartzHelper.BuildTrigger(DateTimeOffset.UtcNow.AddSeconds(ts.Value.TotalSeconds));
                    jobMap[StdRetrySetting.ExecutedNumberContextKey] = executedNumber;
                    var job = QuartzHelper.BuildJob(jobType, jobMap);
                    await context.Scheduler.ScheduleJob(job, trigger).ConfigureAwait(false);
                }
            }
            var executedInfo = new RetryJobExecutedInfo()
            {
                JobStatus = jobStatus,
                FireTimeUtc = context.FireTimeUtc,
                ScheduledFireTimeUtc = context.ScheduledFireTimeUtc,
                JobType = jobType,
                ExecutedNumber = executedNumber,
                JobMap = executedJobMap,
                StoredInfoId = storeId,
                UsedRuleName = ruleName
            };
            this.JobExecuted?.Invoke(executedInfo);
        }
    }
}
