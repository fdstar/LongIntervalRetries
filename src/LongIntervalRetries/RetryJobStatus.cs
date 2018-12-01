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
        /// <summary>
        /// 未完成已杀死，根据设置的Death Time来确定
        /// </summary>
        Killed = 4,
    }
}
