﻿using LongIntervalRetries.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries
{
    /// <summary>
    /// Job注册信息
    /// </summary>
    public class RetryJobRegisterInfo
    {
        /// <summary>
        /// 当前Context中传递的JobData
        /// </summary>
        public IDictionary<string, object> JobMap { get; set; }
        /// <summary>
        /// 要采用的<see cref="IRetryRule"/>规则，若不设定，则采用第一条规则
        /// </summary>
        public string UsedRuleName { get; set; }
    }
}