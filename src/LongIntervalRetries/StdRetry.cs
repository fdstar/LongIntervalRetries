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

using LongIntervalRetries.Rules;
using LongIntervalRetries.Stores;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LongIntervalRetries
{
    /// <summary>
    /// IRetry的标准实现
    /// </summary>
    public class StdRetry : StdRetry<long>
    {
        /// <summary>
        /// 默认构造实现
        /// </summary>
        /// <param name="scheduler"><see cref="IScheduler"/></param>
        /// <param name="store"><see cref="IStore{TKey}"/></param>
        /// <param name="ruleManager"><see cref="IRetryRuleManager"/></param>
        /// <param name="retryJobListener"><see cref="IRetryJobListener"/></param>
        public StdRetry(IScheduler scheduler = null, IStore<long> store = null, IRetryRuleManager ruleManager = null, IRetryJobListener retryJobListener = null)
            : base(scheduler, store, ruleManager, retryJobListener)
        {
        }
    }
    /// <summary>
    /// IRetry的标准实现
    /// </summary>
    /// <typeparam name="TKey"><see cref="Stores.StoredExecutedInfo{TKey}.Id"/></typeparam>
    public class StdRetry<TKey> : IRetry
    {
        private IScheduler _scheduler;
        private IStore<TKey> _store;
        private bool _isNoneStore;
        private ConcurrentDictionary<string, RetryJobExecuted> _events = new ConcurrentDictionary<string, RetryJobExecuted>();
        private ConcurrentDictionary<string, IDictionary<string, Tuple<Type, Func<RetryJobExecutedInfo, RetryJobRegisterInfo>>>> _continues
            = new ConcurrentDictionary<string, IDictionary<string, Tuple<Type, Func<RetryJobExecutedInfo, RetryJobRegisterInfo>>>>();
        //重启恢复时在Quartz还没启动就因为各种原因被判断结束了的Job集合
        private IList<RetryJobExecutedInfo> _endWhileRecover = new List<RetryJobExecutedInfo>();

        /// <summary>
        /// 当前重试规则管理器
        /// </summary>
        public IRetryRuleManager RuleManager { get; private set; }
        /// <summary>
        /// 默认构造实现
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="store"></param>
        /// <param name="ruleManager"></param>
        /// <param name="retryJobListener"></param>
        public StdRetry(IScheduler scheduler = null, IStore<TKey> store = null, IRetryRuleManager ruleManager = null, IRetryJobListener retryJobListener = null)
        {
            this._scheduler = scheduler;
            this.RuleManager = ruleManager;
            this._store = store;
            this.Init(retryJobListener);
        }
        private async void Init(IRetryJobListener retryJobListener)
        {
            if (_scheduler == null)
            {
                this._scheduler = await StdSchedulerFactory.GetDefaultScheduler().ConfigureAwait(false);
            }
            if (RuleManager == null)
            {
                this.RuleManager = new StdRetryRuleManager();
            }
            if (_store == null)
            {
                this._store = new NoneStore<TKey>();
            }
            if (retryJobListener == null)
            {
                retryJobListener = new StdRetryJobListener(this.RuleManager);
            }
            _isNoneStore = this._store is NoneStore<TKey>;
            retryJobListener.JobExecuted += JobListener_JobExecuted;
            this._scheduler.ListenerManager.AddJobListener(retryJobListener, GroupMatcher<JobKey>.GroupEquals(StdRetrySetting.RetryGroupName));
            await this.RecoverRetries().ConfigureAwait(false);
        }
        private async Task RecoverRetries()
        {
            var list = await this._store.GetAllUnfinishedRetries().ConfigureAwait(false);
            if (list != null)
            {
                foreach (var info in list.Where(s => s.JobStatus == RetryJobStatus.Continue))
                {
                    await this.RegisterJob(info).ConfigureAwait(false);
                }
            }
        }
        private async void JobListener_JobExecuted(RetryJobExecutedInfo executedInfo)
        {
            await this._store.Executed(this.GetStoredInfo(executedInfo)).ConfigureAwait(false);
            var key = this.GetJobIdentity(executedInfo.JobType);
            if (this._events.ContainsKey(key))
            {
                this._events[key]?.Invoke(executedInfo);
            }
            if (this._continues.ContainsKey(key))
            {
                var dic = this._continues[key];
                foreach (var k in dic.Keys.Where(k => k.EndsWith(string.Format("_{0}", (int)executedInfo.JobStatus))))
                {
                    var tuple = dic[k];
                    var regInfo = tuple.Item2?.Invoke(executedInfo);
                    if (regInfo != null)
                    {
                        await this.RegisterJob(regInfo, tuple.Item1).ConfigureAwait(false);
                    }
                }
            }
        }
        private StoredInfo<TKey> GetStoredInfo(RetryJobExecutedInfo executedInfo)
        {
            return new StoredInfo<TKey>
            {
                JobStatus = executedInfo.JobStatus,
                JobMap = executedInfo.JobMap,
                JobType = executedInfo.JobType,
                UsedRuleName = executedInfo.UsedRuleName,
                ExecutedNumber = executedInfo.ExecutedNumber,
                Id = (TKey)executedInfo.StoredInfoId,
                PreviousFireTimeUtc = this.GetFireTimeUtcForStore(executedInfo),
                Exception = executedInfo.Exception
            };
        }
        /// <summary>
        /// 获取用于持久化时存储的Job执行时间
        /// </summary>
        /// <param name="executedInfo"></param>
        /// <returns></returns>
        protected virtual DateTimeOffset GetFireTimeUtcForStore(RetryJobExecutedInfo executedInfo)
        {
            return executedInfo.FireTimeUtc;
        }
        /// <summary>
        /// 获取RetryJobExecuted注册的对应IJob应当如何获取唯一性标志
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual string GetJobIdentity(Type type)
        {
            return type.FullName;
        }
        /// <summary>
        /// 注册处理事件
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <param name="event"></param>
        public void RegisterEvent<TJob>(RetryJobExecuted @event) where TJob : IJob
        {
            if (@event == null)
            {
                return;
            }
            this._events[this.GetJobIdentity(typeof(TJob))] = @event;
        }
        /// <summary>
        /// 注册要执行的Job，注意注册成功时此处不判断<see cref="IRetryRule"/>获取的TimeSpan是否小于TimeSpan.Zero，即注册成功的IJob必定会被执行
        /// 但此处会判断EndAt是否大于StartAt或大于当前时间，如果判断不通过，则Job注册失败
        /// 另外如果存在数据持久化，那么外部需捕获持久化异常，目前存储异常不会返回false，而是抛出异常
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <param name="registerInfo"></param>
        public async Task<bool> RegisterJob<TJob>(RetryJobRegisterInfo registerInfo) where TJob : IJob
        {
            return await RegisterJob(registerInfo, typeof(TJob)).ConfigureAwait(false);
        }
        /// <summary>
        /// 注册要执行的Job
        /// </summary>
        /// <param name="registerInfo"></param>
        /// <param name="jobType"></param>
        internal virtual async Task<bool> RegisterJob(RetryJobRegisterInfo registerInfo, Type jobType)
        {
            if ((registerInfo.EndAt.HasValue && registerInfo.EndAt <= DateTimeOffset.Now)
                || (registerInfo.StartAt ?? DateTimeOffset.Now) > registerInfo.EndAt)
            {
                return false;
            }
            StoredInfo<TKey> storedInfo = new StoredInfo<TKey>()
            {
                JobStatus = RetryJobStatus.Continue,
                JobMap = registerInfo.JobMap,
                PreviousFireTimeUtc = registerInfo.StartAt,
                JobType = jobType,
                UsedRuleName = registerInfo.UsedRuleName,
                DeathTimeUtc = registerInfo.EndAt
            };
            storedInfo.Id = await this._store.InsertAndGetId(storedInfo).ConfigureAwait(false);
            if (_isNoneStore || (storedInfo.Id != null && !storedInfo.Id.Equals(default(TKey))))
            {
                await this.RegisterJob(storedInfo).ConfigureAwait(false);
                return true;
            }
            return false;
        }
        private async Task RegisterJob(StoredInfo<TKey> storedInfo)
        {
            var ts = this.RuleManager.GetRule(storedInfo.UsedRuleName)?.GetNextFireSpan(storedInfo.ExecutedNumber);
            var startTime = storedInfo.PreviousFireTimeUtc?.AddSeconds(ts?.TotalSeconds ?? 0);
            if (!startTime.HasValue || startTime < DateTimeOffset.UtcNow)
            {
                startTime = DateTimeOffset.UtcNow;
            }
            if (storedInfo.DeathTimeUtc.HasValue && startTime > storedInfo.DeathTimeUtc)
            {
                this._endWhileRecover.Add(new RetryJobExecutedInfo
                {
                    ExecutedNumber = storedInfo.ExecutedNumber,
                    FireTimeUtc = storedInfo.PreviousFireTimeUtc ?? DateTime.MaxValue,
                    JobMap = storedInfo.JobMap,
                    JobStatus = RetryJobStatus.Killed,
                    JobType = storedInfo.JobType,
                    ScheduledFireTimeUtc = storedInfo.PreviousFireTimeUtc,
                    StoredInfoId = storedInfo.Id,
                    UsedRuleName = storedInfo.UsedRuleName
                });
                return;
            }
            var jobMap = new JobDataMap(storedInfo.JobMap);
            jobMap[StdRetrySetting.ExecutedNumberContextKey] = storedInfo.ExecutedNumber;
            jobMap[StdRetrySetting.RetryRuleNameContextKey] = storedInfo.UsedRuleName;
            jobMap[StdRetrySetting.RetryStoredInfoIdContextKey] = storedInfo.Id;
            jobMap[StdRetrySetting.ExecutedDeathTimeContextKey] = storedInfo.DeathTimeUtc;
            var job = QuartzHelper.BuildJob(storedInfo.JobType, jobMap);
            var trigger = QuartzHelper.BuildTrigger(startTime, storedInfo.DeathTimeUtc);
            await _scheduler.ScheduleJob(job, trigger).ConfigureAwait(false);
        }
        /// <summary>
        /// 启动Job执行
        /// </summary>
        public async void Start()
        {
            foreach (var info in this._endWhileRecover)
            {
                this.JobListener_JobExecuted(info);
            }
            await this._scheduler.Start().ConfigureAwait(false);
        }
        /// <summary>
        /// 停止执行
        /// </summary>
        public async void Stop()
        {
            await this._scheduler.Shutdown().ConfigureAwait(false);
        }
        /// <summary>
        /// 在执行完指定PrevJob后继续执行NextJob
        /// </summary>
        /// <typeparam name="PrevJob"></typeparam>
        /// <typeparam name="NextJob"></typeparam>
        /// <param name="regFunc">如何获取注册信息，如果返回null则表示不执行</param>
        /// <param name="continueParams">需要继续执行的<see cref="RetryJobStatus"/>，每种状态只能注册一个委托，重复注册会替换掉已经注册的委托，默认为<see cref="RetryJobStatus.Completed"/></param>
        /// <returns></returns>
        public void ContinueWith<PrevJob, NextJob>(Func<RetryJobExecutedInfo, RetryJobRegisterInfo> regFunc, params RetryJobStatus[] continueParams)
            where PrevJob : IJob
            where NextJob : IJob
        {
            if (regFunc == null)
            {
                return;
            }
            if (continueParams == null || continueParams.Length == 0)
            {
                continueParams = new RetryJobStatus[] { RetryJobStatus.Completed };
            }
            var prevKey = this.GetJobIdentity(typeof(PrevJob));
            var nextType = typeof(NextJob);
            var nextKey = this.GetJobIdentity(nextType);
            IDictionary<string, Tuple<Type, Func<RetryJobExecutedInfo, RetryJobRegisterInfo>>> dic = new Dictionary<string, Tuple<Type, Func<RetryJobExecutedInfo, RetryJobRegisterInfo>>>();
            if (this._continues.ContainsKey(prevKey))
            {
                dic = this._continues[prevKey];
            }
            else
            {
                this._continues[prevKey] = dic;
            }
            foreach (var status in continueParams)
            {
                var key = string.Format("{0}_{1}", nextKey, (int)status);
                dic[key] = Tuple.Create(nextType, regFunc);
            }
        }
    }
}
