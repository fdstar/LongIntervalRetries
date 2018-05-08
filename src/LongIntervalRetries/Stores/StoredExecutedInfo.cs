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

using LongIntervalRetries.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries.Stores
{
    /// <summary>
    /// 存储的已执行信息
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class StoredExecutedInfo<TKey>
    {
        /// <summary>
        /// 唯一性标志
        /// </summary>
        public TKey Id { get; set; }
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
