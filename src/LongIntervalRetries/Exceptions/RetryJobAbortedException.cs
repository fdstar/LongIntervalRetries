using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries.Exceptions
{
    /// <summary>
    /// 指定当前Job应该终止
    /// </summary>
    public class RetryJobAbortedException : Exception
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public RetryJobAbortedException() : base()
        {
        }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public RetryJobAbortedException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified</param>
        public RetryJobAbortedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
