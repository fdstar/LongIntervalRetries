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
    /// Job注册信息
    /// </summary>
    public class RetryJobRegisterInfo
    {
        /// <summary>
        /// 当前Context中传递的JobData
        /// </summary>
        public IDictionary<string, object> JobMap { get; set; } = new Dictionary<string, object>();
        /// <summary>
        /// 要采用的<see cref="IRetryRule"/>规则，若不设定，则采用第一条规则
        /// </summary>
        public string UsedRuleName { get; set; }
        /// <summary>
        /// 首次开始运行时间，不赋值则代表立刻运行
        /// </summary>
        public DateTimeOffset? StartAt { get; set; }
        /// <summary>
        /// 结束运行时间，将与<see cref="IRetryRule.GetNextFireSpan(int)"/>竞争
        /// </summary>
        public DateTimeOffset? StopAt { get; set; }
    }
}
