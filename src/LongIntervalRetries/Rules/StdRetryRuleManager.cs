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
            if (string.IsNullOrWhiteSpace(name))
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
