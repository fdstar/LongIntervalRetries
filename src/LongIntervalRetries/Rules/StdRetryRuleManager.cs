using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LongIntervalRetries.Rules
{
    /// <summary>
    /// IRetryRuleManager的标准实现
    /// </summary>
    public class StdRetryRuleManager : IRetryRuleManager
    {
        private Dictionary<string, IRetryRule> _dictionary = new Dictionary<string, IRetryRule>();
        /// <summary>
        /// 根据<see cref="IRetryRule.Name"/>获取对应的Rule，如果传入为null，则返回默认第一个IRetryRule，否则返回name对应的IRetryRule，如未找到则返回null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IRetryRule GetRule(string name = null)
        {
            if (name == null)
            {
                return this._dictionary.FirstOrDefault().Value;
            }
            if (this._dictionary.ContainsKey(name))
            {
                return this._dictionary[name];
            }
            return null;
        }
        /// <summary>
        /// 添加IRetryRule
        /// </summary>
        /// <param name="rule"></param>
        public void AddRule(IRetryRule rule)
        {
            this._dictionary[rule.Name] = rule;
        }

        /// <summary>
        /// Removes the IRetryRule with the specified name
        /// </summary>
        /// <param name="name"><see cref="IRetryRule.Name"/></param>
        /// <returns>true if the element is successfully found and removed; otherwise, false. This method returns false if name is not found</returns>
        public bool RemoveRule(string name)
        {
            return this._dictionary.Remove(name);
        }
    }
}
