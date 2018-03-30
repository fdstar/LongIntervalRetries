using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries
{
    /// <summary>
    /// 用于监控重试的<see cref="IJobListener"/>
    /// </summary>
    public interface IRetryJobListener : IJobListener
    {
        /// <summary>
        /// Job执行后的触发事件
        /// </summary>
        event RetryJobExecuted JobExecuted;
    }
}
