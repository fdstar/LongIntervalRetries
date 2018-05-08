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
    /// <see cref="StdRetry"/>相关参数
    /// </summary>
    public class StdRetrySetting
    {
        /// <summary>
        /// 重试Job以及Trigger所属的Group
        /// </summary>
        public static string RetryGroupName { get; set; } = "LongIntervalRetries.RetryGroupName";
        /// <summary>
        /// JobContext中用来传递已经执行了多少次的JobDataMap.Key
        /// </summary>
        public static string ExecutedNumberContextKey { get; set; } = "LongIntervalRetries.ExecutedNumberContextKey";
        /// <summary>
        /// JobContext传递的要采用的<see cref="IRetryRule"/>对应的JobDataMap.Key
        /// </summary>
        public static string RetryRuleNameContextKey { get; set; } = "LongIntervalRetries.RetryRuleNameContextKey";
        /// <summary>
        /// JobContext传递的<see cref="Stores.StoredExecutedInfo{TKey}.Id"/>对应的JobDataMap.Key
        /// </summary>
        public static string RetryStoredInfoIdContextKey { get; set; } = "LongIntervalRetries.RetryStoredInfoIdContextKey";
    }
}
