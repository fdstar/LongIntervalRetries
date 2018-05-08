﻿#region License
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
    /// 自定义重试间隔的重试机制
    /// </summary>
    public class CustomIntervalRetryRule : IRetryRule
    {
        private TimeSpan[] _intervals;
        /// <summary>
        /// 默认构造实现,注意<see cref="GetNextFireSpan(int)"/>会按索引从intervals中获取对应TimeSpan，如未找到则返回TimeSpan.MinValue
        /// </summary>
        /// <param name="name">该RetryRule的唯一性名称</param>
        /// <param name="intervals">自定义的时间间隔</param>
        public CustomIntervalRetryRule(string name, params TimeSpan[] intervals)
        {
            if (intervals == null || intervals.Length == 0)
            {
                throw new ArgumentNullException("intervals can not be empty");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name can not be empty");
            }
            this._intervals = intervals;
            this.Name = name;
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
            if (executedNumber >= 0 && executedNumber < this._intervals.Length)
            {
                return this._intervals[executedNumber];
            }
            return TimeSpan.MinValue;
        }
    }
}
