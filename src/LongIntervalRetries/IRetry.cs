using LongIntervalRetries.Rules;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LongIntervalRetries
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
        /// <typeparam name="TJob"><see cref="IJob"/></typeparam>
        /// <param name="event"></param>
        void RegisterEvent<TJob>(RetryJobExecuted @event) where TJob : IJob;
        /// <summary>
        /// 注册要执行的Job
        /// </summary>
        /// <typeparam name="TJob"><see cref="IJob"/></typeparam>
        /// <param name="registerInfo"></param>
        Task RegisterJob<TJob>(RetryJobRegisterInfo registerInfo) where TJob : IJob;
        /// <summary>
        /// 注册要执行的Job
        /// </summary>
        /// <param name="registerInfo"></param>
        /// <param name="jobType"></param>
        Task RegisterJob(RetryJobRegisterInfo registerInfo, Type jobType);
        /// <summary>
        /// 启动Job执行
        /// </summary>
        void Start();
        /// <summary>
        /// 停止执行
        /// </summary>
        void Stop();
    }
}
