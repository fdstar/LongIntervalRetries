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

namespace LongIntervalRetries
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
        /// <see cref="Stores.StoredExecutedInfo{TKey}.Id"/>
        /// </summary>
        internal object StoredInfoId { get; set; }
        /// <summary>
        /// 要采用的<see cref="IRetryRule"/>规则，若不设定，则采用第一条规则
        /// </summary>
        internal string UsedRuleName { get; set; }
        /// <summary>
        /// 当前Context中传递的JobData
        /// </summary>
        public IDictionary<string, object> JobMap { get; set; }
        /// <summary>
        /// 是否需要持久化<see cref="JobMap"/>数据
        /// </summary>
        internal bool PersistJobData { get; set; }
        /// <summary>
        /// Job计划执行时间
        /// </summary>
        public DateTimeOffset? ScheduledFireTimeUtc { get; set; }
        /// <summary>
        /// Job实际执行时间
        /// </summary>
        public DateTimeOffset FireTimeUtc { get; set; }
        /// <summary>
        /// Job执行时产生的异常
        /// </summary>
        public Exception Exception { get; set; }
    }
}
