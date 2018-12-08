#region License
/* 
 * All content copyright Dong Fang.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not 
 * use this file except in compliance with the License. You may obtain a copy 
 * of the License at 
 * 
 *   http://www.apache.org/licenses/LICENSE-2.0 
 *   
 * Unless required by applicable law or agreed to in writing, software 
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations 
 * under the License.
 * 
 */
#endregion

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
        private RetryJobExecutedInfo GetExecutedInfoWithoutStatus(IJobExecutionContext context,out JobDataMap jobMap)
        {
            jobMap = context.JobDetail.JobDataMap;
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
            var removeKeies = new string[] { StdRetrySetting.ExecutedNumberContextKey, StdRetrySetting.RetryRuleNameContextKey, StdRetrySetting.RetryStoredInfoIdContextKey, StdRetrySetting.ExecutedDeathTimeContextKey };
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
            var ruleName = jobMap.GetString(StdRetrySetting.RetryRuleNameContextKey);
            var executedInfo = new RetryJobExecutedInfo()
            {
                FireTimeUtc = context.FireTimeUtc,
                ScheduledFireTimeUtc = context.ScheduledFireTimeUtc,
                JobType = jobType,
                ExecutedNumber = executedNumber,
                JobMap = executedJobMap,
                StoredInfoId = storeId,
                UsedRuleName = ruleName,
                PersistJobData = persistJobData
            };
            return executedInfo;
        }
        private async void Deal(IJobExecutionContext context, JobExecutionException jobException)
        {
            var executedInfo = this.GetExecutedInfoWithoutStatus(context, out JobDataMap jobMap);
            var deathTime = jobMap.Get(StdRetrySetting.ExecutedDeathTimeContextKey) as DateTimeOffset?;
            var jobStatus = RetryJobStatus.Completed;
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
                    var ts = this._ruleManager.GetRule(executedInfo.UsedRuleName)?.GetNextFireSpan(executedInfo.ExecutedNumber);
                    if (ts >= TimeSpan.Zero)
                    {
                        var startAt = DateTimeOffset.UtcNow.AddSeconds(ts.Value.TotalSeconds);
                        jobStatus = RetryJobStatus.Killed;
                        if (!deathTime.HasValue || deathTime >= startAt)
                        {
                            jobStatus = RetryJobStatus.Continue;
                            var trigger = QuartzHelper.BuildTrigger(startAt, deathTime);
                            jobMap[StdRetrySetting.ExecutedNumberContextKey] = executedInfo.ExecutedNumber;
                            var job = QuartzHelper.BuildJob(executedInfo.JobType, jobMap);
                            await context.Scheduler.ScheduleJob(job, trigger).ConfigureAwait(false);
                        }
                    }
                }
            }
            executedInfo.JobStatus = jobStatus;
            this.JobExecuted?.Invoke(executedInfo);
        }
    }
}
