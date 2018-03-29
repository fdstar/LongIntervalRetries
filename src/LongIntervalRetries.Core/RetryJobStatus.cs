using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries.Core
{
    /// <summary>
    /// 当前Job执行状态
    /// </summary>
    public enum RetryJobStatus
    {
        /// <summary>
        /// 继续执行
        /// </summary>
        Continue = 0,
        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 1,
        /// <summary>
        /// 未完成已取消
        /// </summary>
        Canceled = 2
    }
}
