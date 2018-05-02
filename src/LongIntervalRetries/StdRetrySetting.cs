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
