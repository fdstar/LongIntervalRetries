using Quartz.Listener;
using System;
using System.Collections.Generic;
using System.Text;
using Quartz;
using System.Threading;
using System.Threading.Tasks;
using LongIntervalRetries.Rules;
using LongIntervalRetries.Exceptions;
using System.Collections.Concurrent;

namespace LongIntervalRetries
{
    internal class StdRetryJobListener : JobListenerSupport, IRetryJobListener
    {
        private IRetryRuleManager _ruleManager;
        private ConcurrentDictionary<Type, bool> _dictionary = new ConcurrentDictionary<Type, bool>();
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
        private bool ContainsPersistJobDataAfterExecutionAttribute(Type jobType)
        {
            var attrs = jobType.GetCustomAttributes(typeof(PersistJobDataAfterExecutionAttribute), false);
            return attrs != null && attrs.Length > 0;
        }
        private async void Deal(IJobExecutionContext context, JobExecutionException jobException)
        {
            var jobMap = context.JobDetail.JobDataMap;
            var jobType = context.JobDetail.JobType;
            if (!_dictionary.ContainsKey(jobType))
            {
                var contains = ContainsPersistJobDataAfterExecutionAttribute(jobType);
                _dictionary.AddOrUpdate(jobType, contains, (t, v) => contains);
            }
            var persistJobData = _dictionary[jobType];
            if (persistJobData)
            {
                jobMap = context.MergedJobDataMap;
            }
            var executedNumber = jobMap.GetIntValue(StdRetrySetting.ExecutedNumberContextKey) + 1;
            if (executedNumber < 1) executedNumber = 1;
            var removeKeies = new string[] { StdRetrySetting.ExecutedNumberContextKey, StdRetrySetting.RetryRuleNameContextKey, StdRetrySetting.RetryStoredInfoIdContextKey };
            var executedJobMap = new Dictionary<string, object>(jobMap);
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
                if (jobException.InnerException?.InnerException != null
                    && jobException.InnerException?.InnerException is RetryJobAbortedException)
                {
                    jobStatus = RetryJobStatus.Aborted;
                }
                else
                {
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
                UsedRuleName = ruleName,
                PersistJobData = persistJobData
            };
            this.JobExecuted?.Invoke(executedInfo);
        }
    }
}
