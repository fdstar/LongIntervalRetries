using LongIntervalRetries.Core.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries.Core
{
    /// <summary>
    /// Job注册信息
    /// </summary>
    public class RetryJobRegisterInfo
    {
        /// <summary>
        /// 当前已执行次数
        /// </summary>
        public int ExecutedNumber { get; set; }
        /// <summary>
        /// Job开始执行时间，注意实际Job执行时间会在此基础上加上<see cref="IRetryRule.GetNextFireSpan(int)"/>时间
        /// </summary>
        public DateTimeOffset? StartAtTimeUtc { get; set; }
        /// <summary>
        /// 当前Context中传递的JobData
        /// </summary>
        public IDictionary<string, object> JobMap { get; set; }
        /// <summary>
        /// 当前执行的Job类型
        /// </summary>
        public Type JobType { get; set; }
        /// <summary>
        /// 要采用的<see cref="IRetryRule"/>规则，若不设定，则采用第一条规则
        /// </summary>
        public string UsedRuleName { get; set; }
    }
}
