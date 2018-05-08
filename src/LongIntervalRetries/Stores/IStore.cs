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
using System.Text;
using System.Threading.Tasks;

namespace LongIntervalRetries.Stores
{
    /// <summary>
    /// 存储定义
    /// </summary>
    /// <typeparam name="TKey">唯一性标志类型</typeparam>
    public interface IStore<TKey>
    {
        /// <summary>
        /// 获取所有尚未结束需重试执行的任务
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<StoredInfo<TKey>>> GetAllUnfinishedRetries();
        /// <summary>
        /// 添加Job信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TKey> InsertAndGetId(StoredInfo<TKey> entity);
        /// <summary>
        /// 更新执行信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task Executed(StoredExecutedInfo<TKey> entity);
    }
}
