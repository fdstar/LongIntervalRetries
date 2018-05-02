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
        /// 在执行完指定PrevJob后注册执行NextJob
        /// </summary>
        /// <typeparam name="PrevJob"></typeparam>
        /// <typeparam name="NextJob"></typeparam>
        /// <param name="regFunc">如何获取注册信息，如果返回null则表示不执行</param>
        /// <param name="continueParams">需要继续执行的<see cref="RetryJobStatus"/>，每种状态只能注册一个委托，重复注册会替换掉已经注册的委托，默认为<see cref="RetryJobStatus.Completed"/></param>
        /// <returns></returns>
        void ContinueWith<PrevJob, NextJob>(Func<RetryJobExecutedInfo, RetryJobRegisterInfo> regFunc, params RetryJobStatus[] continueParams)
            where PrevJob : IJob
            where NextJob : IJob;
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
