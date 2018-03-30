using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries.Rules
{
    /// <summary>
    /// 简单重复时间间隔重试机制
    /// </summary>
    public class SimpleRepeatRetryRule : IRetryRule
    {
        private int _maxExecutedNumber;
        private TimeSpan _timeSpan;
        /// <summary>
        /// 默认构造实现
        /// </summary>
        /// <param name="name">该RetryRule的唯一性名称</param>
        /// <param name="maxExecutedNumber">允许的最大重试次数（含）</param>
        /// <param name="timeSpan"></param>
        public SimpleRepeatRetryRule(string name, int maxExecutedNumber, TimeSpan timeSpan)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name can not be empty");
            }
            this.Name = name;
            this._maxExecutedNumber = maxExecutedNumber;
            this._timeSpan = timeSpan;
        }
        /// <summary>
        /// Get the name of the IRetryRule
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 根据已经执行的次数获取下一次执行时间间隔，返回小于TimeSpan.Zero的值表示不再需要执行
        /// </summary>
        /// <param name="executedNumber"></param>
        /// <returns></returns>
        public TimeSpan GetNextFireSpan(int executedNumber)
        {
            if (executedNumber >= this._maxExecutedNumber)
            {
                return TimeSpan.MinValue;
            }
            return this._timeSpan;
        }
    }
}
