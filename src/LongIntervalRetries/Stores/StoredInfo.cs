using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries.Stores
{
    /// <summary>
    /// 存储的Job执行信息
    /// </summary>
    public class StoredInfo<T>
        : RetryJobRegisterInfo
    {
        /// <summary>
        /// 唯一性标志
        /// </summary>
        public T Id { get; set; }
        /// <summary>
        /// 对应的<see cref="Quartz.IJob"/>
        /// </summary>
        public Type JobType { get; set; }
        /// <summary>
        /// 当前Job状态，可以根据状态进行相应的业务处理
        /// </summary>
        public RetryJobStatus JobStatus { get; set; }
    }
}
