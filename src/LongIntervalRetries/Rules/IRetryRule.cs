using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries.Rules
{
    /// <summary>
    /// 重试规则
    /// </summary>
    public interface IRetryRule
    {
        /// <summary>
        /// Get the name of the IRetryRule
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 根据已经执行的次数获取下一次执行时间间隔，返回小于TimeSpan.Zero的值表示不再需要执行
        /// </summary>
        /// <param name="executedNumber">已经执行的次数</param>
        /// <returns></returns>
        TimeSpan GetNextFireSpan(int executedNumber);
    }
}
