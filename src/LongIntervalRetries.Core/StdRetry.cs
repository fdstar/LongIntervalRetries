using LongIntervalRetries.Core.Rules;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries.Core
{
    /// <summary>
    /// IRetry的标准实现
    /// </summary>
    public class StdRetry : IRetry
    {
        private IScheduler _scheduler;
        private IRetryRuleManager _ruleManager;
        private Dictionary<string, RetryJobExecuted> _events = new Dictionary<string, RetryJobExecuted>();
        /// <summary>
        /// 重试Job以及Trigger所属的Group
        /// </summary>
        public static string RetryGroupName { get; set; } = "LongIntervalRetries.RetryGroupName";
        /// <summary>
        /// JobContext中用来传递已经执行了多少次的JobDataMap.Key
        /// </summary>
        public static string ExecutedNumberContextKey { get; set; } = "LongIntervalRetries.ExecutedNumberContextKey";
        /// <summary>
        /// JobContext传递的要采用的<see cref="IRetryRule"/>对应的JobDataMap.Key
        /// </summary>
        public static string RetryRuleNameContextKey { get; set; } = "LongIntervalRetries.RetryRuleNameContextKey";
        /// <summary>
        /// 当前重试规则管理器
        /// </summary>
        public IRetryRuleManager RuleManager
        {
            get { return this._ruleManager; }
        }
        /// <summary>
        /// 默认构造实现
        /// </summary>
        /// <param name="scheduler"></param>
        public StdRetry(IScheduler scheduler = null)
        {
            this._scheduler = scheduler;
            this.Init();
        }
        private async void Init()
        {
            if (_scheduler == null)
            {
                _scheduler = await StdSchedulerFactory.GetDefaultScheduler().ConfigureAwait(false);
            }
            this._ruleManager = new StdRetryRuleManager();
            var jobListener = new StdRetryJobListener(this._ruleManager);
            jobListener.JobExecuted += JobListener_JobExecuted;
            this._scheduler.ListenerManager.AddJobListener(jobListener, GroupMatcher<JobKey>.GroupEquals(RetryGroupName));
        }

        private void JobListener_JobExecuted(RetryJobExecutedInfo executedInfo)
        {
            var key = this.GetEventIdentity(executedInfo.JobType);
            if (this._events.ContainsKey(key))
            {
                this._events[key]?.Invoke(executedInfo);
            }
        }
        /// <summary>
        /// 获取RetryJobExecuted注册的对应IJob应当如何获取唯一性标志
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual string GetEventIdentity(Type type)
        {
            return type.FullName;
        }

        /// <summary>
        /// 注册处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        public void RegisterEvent<T>(RetryJobExecuted @event) where T : IJob
        {
            this._events[this.GetEventIdentity(typeof(T))] = @event;
        }
        /// <summary>
        /// 注册要执行的Job，注意此处不判断<see cref="IRetryRule"/>获取的TimeSpan是否小于TimeSpan.Zero，即注册的IJob必定会被执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registerInfo"></param>
        public void RegisterJob<T>(RetryJobRegisterInfo registerInfo) where T : IJob
        {
            var ts = this._ruleManager.GetRule(registerInfo.UsedRuleName)?.GetNextFireSpan(registerInfo.ExecutedNumber);
            var startTime = (registerInfo.StartAtTimeUtc ?? DateTimeOffset.UtcNow).AddSeconds(ts?.TotalSeconds ?? 0);
            if (startTime < DateTimeOffset.UtcNow)
            {
                startTime = DateTimeOffset.UtcNow;
            }
            var jobMap = new JobDataMap(registerInfo.JobMap);
            jobMap[ExecutedNumberContextKey] = registerInfo.ExecutedNumber;
            jobMap[RetryRuleNameContextKey] = registerInfo.UsedRuleName;
            var job = QuartzHelper.BuildJob<T>(jobMap);
            var trigger = QuartzHelper.BuildTrigger(startTime);
            _scheduler.ScheduleJob(job, trigger);
        }
    }
}
