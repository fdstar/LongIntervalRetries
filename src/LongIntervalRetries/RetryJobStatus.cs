using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries
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
        /// 未完成已取消，表示已达到最大重试次数
        /// </summary>
        Canceled = 2,
        /// <summary>
        /// 由job给定异常指定终止
        /// </summary>
        Aborted = 3,
    }
}
