using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries.Samples.Jobs
{
    public static class RandomHelper
    {
        public static int Next(this int maxNumber)
        {
            if(maxNumber<=0)
            {
                throw new ArgumentException();
            }
            return Math.Abs(Guid.NewGuid().GetHashCode()) % maxNumber;
        }
    }
}
