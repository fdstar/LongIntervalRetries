using LongIntervalRetries.Core.Rules;
using Quartz;
using Quartz.Impl;
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
        /// <summary>
        /// Job已执行事件
        /// </summary>
        public event RetryJobExecuted JobExecuted;
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
        }
    }
}
