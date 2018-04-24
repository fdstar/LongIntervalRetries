using LongIntervalRetries.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries.Stores.AdoStores.Entities
{
    /// <summary>
    /// 重试存储信息
    /// </summary>
    public class RetryStore
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }
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
        public DateTime PreviousFireTime { get; set; }
        /// <summary>
        /// 完整的Job全名，包含程序集名称
        /// </summary>
        public string JobTypeName { get; set; }
        /// <summary>
        /// 要采用的<see cref="IRetryRule"/>规则，若不设定，则采用第一条规则
        /// </summary>
        public string UsedRuleName { get; set; }
    }
}
