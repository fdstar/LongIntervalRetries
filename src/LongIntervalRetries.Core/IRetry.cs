using LongIntervalRetries.Core.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries.Core
{
    /// <summary>
    /// Retry规范定义
    /// </summary>
    public interface IRetry
    {
        /// <summary>
        /// 当前重试规则管理器
        /// </summary>
        IRetryRuleManager RuleManager { get; }
    }
}
