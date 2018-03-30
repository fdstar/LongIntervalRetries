using LongIntervalRetries.Core.Rules;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries.Core
{
    /// <summary>
    /// Retry规范定义
    /// </summary>
    public interface IRetry
    {
        /// <summary>
        /// 当前重试规则管理器
        /// </summary>
        IRetryRuleManager RuleManager { get; }
        /// <summary>
        /// 注册处理事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        void RegisterEvent<T>(RetryJobExecuted @event) where T : IJob;
        /// <summary>
        /// 注册要执行的Job
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registerInfo"></param>
        void RegisterJob<T>(RetryJobRegisterInfo registerInfo) where T : IJob;
    }
}
