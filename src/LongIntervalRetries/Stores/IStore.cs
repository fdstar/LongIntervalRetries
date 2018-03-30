using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries.Stores
{
    /// <summary>
    /// 存储定义
    /// </summary>
    public interface IStore<T>
    {
        /// <summary>
        /// 获取所有尚未结束需重试执行的任务
        /// </summary>
        /// <returns></returns>
        IEnumerable<StoredInfo<T>> GetAllUnfinishedRetries();
        /// <summary>
        /// 添加Job
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Add(StoredInfo<T> entity);
        /// <summary>
        /// 更新Job
        /// </summary>
        /// <param name="entit"></param>
        /// <returns></returns>
        bool Update(StoredInfo<T> entit);
    }
}
