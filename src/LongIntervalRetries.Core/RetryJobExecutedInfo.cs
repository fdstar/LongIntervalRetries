using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries.Core
{
    /// <summary>
    /// Job每次执行完毕时的回调委托
    /// </summary>
    /// <param name="executedInfo"></param>
    public delegate void RetryJobExecuted(RetryJobExecutedInfo executedInfo);
    /// <summary>
    /// Job已执行信息
    /// </summary>
    public class RetryJobExecutedInfo
    {
        /// <summary>
        /// 当前已执行次数
        /// </summary>
        public int ExecutedNumber { get; set; }
        /// <summary>
        /// 当前Job状态，可以根据状态进行相应的业务处理
        /// </summary>
        public RetryJobStatus JobStatus { get; set; }
        /// <summary>
        /// 当前执行的Job类型
        /// </summary>
        internal Type JobType { get; set; }
        /// <summary>
        /// 当前Context中传递的JobData
        /// </summary>
        public IDictionary<string, object> JobMap { get; set; }
        /// <summary>
        /// Job计划执行时间
        /// </summary>
        public DateTimeOffset? ScheduledFireTimeUtc { get; set; }
        /// <summary>
        /// Job实际执行时间
        /// </summary>
        public DateTimeOffset FireTimeUtc { get; set; }
    }
}
