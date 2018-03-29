using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries.Core.Rules
{
    /// <summary>
    /// 重试规则管理器
    /// </summary>
    public interface IRetryRuleManager
    {
        /// <summary>
        /// 根据<see cref="IRetryRule.Name"/>获取对应的Rule，如果传入为null，则返回默认第一个IRetryRule，否则返回name对应的IRetryRule，如未找到则返回null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IRetryRule GetRule(string name = null);
        /// <summary>
        /// 添加IRetryRule
        /// </summary>
        /// <param name="rule"></param>
        void AddRule(IRetryRule rule);
        /// <summary>
        /// Removes the IRetryRule with the specified name
        /// </summary>
        /// <param name="name"><see cref="IRetryRule.Name"/></param>
        /// <returns>true if the element is successfully found and removed; otherwise, false. This method returns false if name is not found</returns>
        bool RemoveRule(string name);
    }
}
