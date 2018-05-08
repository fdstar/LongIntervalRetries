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

namespace LongIntervalRetries.Stores.AdoStores.Entities
{
    /// <summary>
    /// 重试相关的上下文数据信息
    /// </summary>
    public class RetryStoreData
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// <see cref="RetryStore.Id"/>
        /// </summary>
        public long RetryStoreId { get; set; }
        /// <summary>
        /// 键值名
        /// </summary>
        public string KeyName { get; set; }
        /// <summary>
        /// 序列化后的数据内容
        /// </summary>
        public string DataContent { get; set; }
        /// <summary>
        /// 完整的值类型全名，包含程序集名称
        /// </summary>
        public string DataTypeName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
