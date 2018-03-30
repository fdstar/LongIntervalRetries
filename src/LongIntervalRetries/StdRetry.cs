using LongIntervalRetries.Rules;
using LongIntervalRetries.Stores;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
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
        /// <param name="scheduler"></param>
        /// <param name="store"></param>
        /// <param name="ruleManager"></param>
        /// <param name="retryJobListener"></param>
        public StdRetry(IScheduler scheduler = null, IStore<long> store = null, IRetryRuleManager ruleManager = null, IRetryJobListener retryJobListener = null)
            : base(scheduler, store, ruleManager, retryJobListener)
        {
        }
    }
    /// <summary>
    /// IRetry的标准实现
    /// </summary>
    /// <typeparam name="TKey"><see cref="Stores.StoredInfo{TKey}.Id"/></typeparam>
    public class StdRetry<TKey> : IRetry
    {
        private IScheduler _scheduler;
        private IRetryRuleManager _ruleManager;
        private IStore<TKey> _store;
        private Dictionary<string, RetryJobExecuted> _events = new Dictionary<string, RetryJobExecuted>();
        
        /// <summary>
        /// 当前重试规则管理器
        /// </summary>
        public IRetryRuleManager RuleManager
        {
            get { return this._ruleManager; }
        }
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
            this._ruleManager = ruleManager;
            this._store = store;
            this.Init(retryJobListener);
        }
        private async void Init(IRetryJobListener retryJobListener)
        {
            if (_scheduler == null)
            {
                this._scheduler = await StdSchedulerFactory.GetDefaultScheduler().ConfigureAwait(false);
            }
            if (_ruleManager == null)
            {
                this._ruleManager = new StdRetryRuleManager();
            }
            if (_store == null)
            {
                this._store = new NoneStore<TKey>();
            }
            if (retryJobListener == null)
            {
                retryJobListener = new StdRetryJobListener(this._ruleManager);
            }
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
            await this._store.Update(this.GetStoredInfo(executedInfo)).ConfigureAwait(false);
            var key = this.GetEventIdentity(executedInfo.JobType);
            if (this._events.ContainsKey(key))
            {
                this._events[key]?.Invoke(executedInfo);
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
        protected virtual string GetEventIdentity(Type type)
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
            this._events[this.GetEventIdentity(typeof(TJob))] = @event;
        }
        /// <summary>
        /// 注册要执行的Job，注意此处不判断<see cref="IRetryRule"/>获取的TimeSpan是否小于TimeSpan.Zero，即注册的IJob必定会被执行
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <param name="registerInfo"></param>
        public async Task RegisterJob<TJob>(RetryJobRegisterInfo registerInfo) where TJob : IJob
        {
            await RegisterJob(registerInfo, typeof(TJob)).ConfigureAwait(false);
        }
        /// <summary>
        /// 注册要执行的Job
        /// </summary>
        /// <param name="registerInfo"></param>
        /// <param name="jobType"></param>
        public virtual async Task RegisterJob(RetryJobRegisterInfo registerInfo, Type jobType)
        {
            StoredInfo<TKey> storedInfo = new StoredInfo<TKey>()
            {
                JobStatus = RetryJobStatus.Continue,
                JobMap = registerInfo.JobMap,
                JobType = jobType,
                UsedRuleName = registerInfo.UsedRuleName,
            };
            storedInfo.Id = await this._store.InsertAndGetId(storedInfo).ConfigureAwait(false);
            await this.RegisterJob(storedInfo).ConfigureAwait(false);
        }
        private async Task RegisterJob(StoredInfo<TKey> storedInfo)
        {
            var ts = this._ruleManager.GetRule(storedInfo.UsedRuleName)?.GetNextFireSpan(storedInfo.ExecutedNumber);
            var startTime = (storedInfo.PreviousFireTimeUtc ?? DateTimeOffset.UtcNow).AddSeconds(ts?.TotalSeconds ?? 0);
            if (startTime < DateTimeOffset.UtcNow)
            {
                startTime = DateTimeOffset.UtcNow;
            }
            var jobMap = new JobDataMap(storedInfo.JobMap);
            jobMap[StdRetrySetting.ExecutedNumberContextKey] = storedInfo.ExecutedNumber;
            jobMap[StdRetrySetting.RetryRuleNameContextKey] = storedInfo.UsedRuleName;
            jobMap[StdRetrySetting.RetryStoredInfoIdContextKey] = storedInfo.Id;
            var job = QuartzHelper.BuildJob(storedInfo.JobType, jobMap);
            var trigger = QuartzHelper.BuildTrigger(startTime);
            await _scheduler.ScheduleJob(job, trigger).ConfigureAwait(false);
        }
        /// <summary>
        /// 启动Job执行
        /// </summary>
        public async void Start()
        {
            await this._scheduler.Start().ConfigureAwait(false);
        }
        /// <summary>
        /// 停止执行
        /// </summary>
        public async void Stop()
        {
            await this._scheduler.Shutdown().ConfigureAwait(false);
        }
    }
}
