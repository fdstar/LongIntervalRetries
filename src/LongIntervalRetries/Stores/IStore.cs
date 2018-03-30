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
        /// 添加Job
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TKey> InsertAndGetId(StoredInfo<TKey> entity);
        /// <summary>
        /// 更新Job
        /// </summary>
        /// <param name="entit"></param>
        /// <returns></returns>
        Task Update(StoredInfo<TKey> entit);
    }
}
