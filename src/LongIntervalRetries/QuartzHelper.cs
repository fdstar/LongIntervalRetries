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

using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace LongIntervalRetries
{
    internal class QuartzHelper
    {
        public static ITrigger BuildTrigger(DateTimeOffset? startAt, string name = null)
        {
            return TriggerBuilder.Create()
                .WithIdentity(name ?? Guid.NewGuid().ToString(), StdRetrySetting.RetryGroupName)
                .StartAt(startAt ?? DateTimeOffset.UtcNow)
                .WithSimpleSchedule(x => x.WithRepeatCount(0))
                .Build();
        }
        public static IJobDetail BuildJob<T>(JobDataMap map, string name = null) where T : IJob
        {
            return BuildJob(typeof(T), map, name);
        }
        public static IJobDetail BuildJob(Type type, JobDataMap map, string name = null)
        {
            return JobBuilder.Create(type)
                .WithIdentity(name ?? Guid.NewGuid().ToString(), StdRetrySetting.RetryGroupName)
                .SetJobData(map)
                .Build();
        }
    }
}
