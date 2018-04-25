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
