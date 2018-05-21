#region License
/* 
 * All content copyright Dong Fang.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not 
 * use this file except in compliance with the License. You may obtain a copy 
 * of the License at 
 * 
 *   http://www.apache.org/licenses/LICENSE-2.0 
 *   
 * Unless required by applicable law or agreed to in writing, software 
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations 
 * under the License.
 * 
 */
#endregion

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
        private TimeSpan _timeSpan;
        /// <summary>
        /// 默认构造实现
        /// </summary>
        /// <param name="name">该RetryRule的唯一性名称</param>
        /// <param name="maxExecutedNumber">允许的最大执行次数（含第一次正常执行+失败重试总次数）</param>
        /// <param name="timeSpan"></param>
        public SimpleRepeatRetryRule(string name, int maxExecutedNumber, TimeSpan timeSpan)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name can not be empty");
            }
            if (maxExecutedNumber < 1)
            {
                throw new ArgumentException("maxExecutedNumber must be 1 or greater");
            }
            if (timeSpan < TimeSpan.Zero)
            {
                throw new ArgumentException("timeSpan must be greater than or equals with TimeSpan.Zero");
            }
            this.Name = name;
            this.MaxExecutedNumber = maxExecutedNumber;
            this._timeSpan = timeSpan;
        }
        /// <summary>
        /// Get the name of the IRetryRule
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 最大执行次数（第一次正常执行+失败重试总次数）
        /// </summary>
        public int MaxExecutedNumber { get; private set; }
        /// <summary>
        /// 根据已经执行的次数获取下一次执行时间间隔，返回小于TimeSpan.Zero的值表示不再需要执行
        /// </summary>
        /// <param name="executedNumber"></param>
        /// <returns></returns>
        public TimeSpan GetNextFireSpan(int executedNumber)
        {
            if (executedNumber <= 0)
            {
                return TimeSpan.Zero;
            }
            else if (executedNumber >= this.MaxExecutedNumber)
            {
                return TimeSpan.MinValue;
            }
            return this._timeSpan;
        }
    }
}
