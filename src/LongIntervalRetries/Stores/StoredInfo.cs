using LongIntervalRetries.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries.Stores
{
    /// <summary>
    /// 存储的Job执行信息
    /// </summary>
    public class StoredInfo<TKey>
        : RetryJobRegisterInfo
    {
        /// <summary>
        /// 唯一性标志
        /// </summary>
        public TKey Id { get; set; }
        /// <summary>
        /// 对应的<see cref="Quartz.IJob"/>
        /// </summary>
        public Type JobType { get; set; }
        /// <summary>
        /// 当前Job状态，可以根据状态进行相应的业务处理
        /// </summary>
        public RetryJobStatus JobStatus { get; set; }
        /// <summary>
        /// 当前已执行次数
        /// </summary>
        public int ExecutedNumber { get; set; }
        /// <summary>
        /// 上一次执行时间，注意实际Job执行时间会在此基础上加上<see cref="IRetryRule.GetNextFireSpan(int)"/>时间
        /// </summary>
        public DateTimeOffset? PreviousFireTimeUtc { get; set; }
    }
}
