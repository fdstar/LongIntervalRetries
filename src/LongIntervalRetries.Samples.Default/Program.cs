using LongIntervalRetries.Rules;
using LongIntervalRetries.Samples.Jobs;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Topshelf;

namespace LongIntervalRetries.Samples.Default
{
    class Program
    {
        protected static readonly Logger Logger = LogManager.GetLogger("Default");
        static void Main(string[] args)
        {
            var retry = new StdRetry();
            string simpleRuleName = "SimpleRepeatRetryRule";
            string customRuleName = "CustomIntervalRetryRule";
            //声明并注册重试规则
            var simpleRepeatRule = new SimpleRepeatRetryRule(simpleRuleName, 5, TimeSpan.FromSeconds(2));
            var customIntervalRule = new CustomIntervalRetryRule(customRuleName,
                Enumerable.Range(1, 6).Select(r => TimeSpan.FromSeconds(r)).ToArray());
            retry.RuleManager.AddRule(simpleRepeatRule);
            retry.RuleManager.AddRule(customIntervalRule);

            var tasks = new Task[] {
                //注册Job
                retry.RegisterJob<AlawaysSuccessJob>(new RetryJobRegisterInfo{ UsedRuleName= simpleRuleName, StartAt= DateTimeOffset.UtcNow.AddSeconds(3)}),
                //retry.RegisterJob<AlawaysFailJob>(new RetryJobRegisterInfo{ UsedRuleName= customRuleName}),
                //retry.RegisterJob<MaybeFailJob>(new RetryJobRegisterInfo{ JobMap=new Dictionary<string,object>{ {"Id",1 } } , UsedRuleName= customRuleName}),
                //retry.RegisterJob<MaybeFailJob>(new RetryJobRegisterInfo{ JobMap=new Dictionary<string,object>{ {"Id",2 } } ,UsedRuleName= customRuleName}),
                //retry.RegisterJob<MaybeFailJob>(new RetryJobRegisterInfo{ JobMap=new Dictionary<string,object>{ {"Id",3 } } ,UsedRuleName= customRuleName}),
                //retry.RegisterJob<MaybeFailJob>(new RetryJobRegisterInfo{ JobMap=new Dictionary<string,object>{ {"Id",4 } } ,UsedRuleName= customRuleName}),
                //retry.RegisterJob<MaybeFailJob>(new RetryJobRegisterInfo{ JobMap=new Dictionary<string,object>{ {"Id",5 } } ,UsedRuleName= "NotSetRule"}),
            };

            retry.ContinueWith<AlawaysSuccessJob, MaybeFailJob>(i => new RetryJobRegisterInfo
            {
                JobMap = new Dictionary<string, object> { { "Id", DateTime.Now.Ticks % int.MaxValue } }
            });

            //注册执行完时的通知事件
            retry.RegisterEvent<AlawaysSuccessJob>(e =>
            {
                Logger.Info("AlawaysSuccessJob Result:{0}", JsonConvert.SerializeObject(e));
            });
            retry.RegisterEvent<AlawaysFailJob>(e =>
            {
                Logger.Info("AlawaysFailJob Result:{0}", JsonConvert.SerializeObject(e));
            });
            retry.RegisterEvent<MaybeFailJob>(e =>
            {
                Logger.Info("MaybeFailJob Result:{0}", JsonConvert.SerializeObject(e));
            });

            var host = HostFactory.New(x =>
            {
                x.Service<StdRetry>(s =>
                {
                    s.ConstructUsing(settings => retry);
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });
            });
            host.Run();
        }
    }
}
