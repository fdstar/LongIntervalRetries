using LongIntervalRetries.Rules;
using LongIntervalRetries.Samples.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Topshelf;

namespace LongIntervalRetries.Samples.Default
{
    class Program
    {
        static void Main(string[] args)
        {
            string simpleRuleName = "SimpleRepeatRetryRule";
            string customRuleName = "CustomIntervalRetryRule";
            var retry = new StdRetry();
            var simpleRepeatRule = new SimpleRepeatRetryRule(simpleRuleName, 5, TimeSpan.FromSeconds(2));
            var customIntervalRule = new CustomIntervalRetryRule(customRuleName,
                Enumerable.Range(1, 6).Select(r => TimeSpan.FromSeconds(r)).ToArray());
            retry.RuleManager.AddRule(simpleRepeatRule);

            var tasks = new Task[] {
                retry.RegisterJob<AlawaysSuccessJob>(new RetryJobRegisterInfo{ UsedRuleName= simpleRuleName}),
                retry.RegisterJob<AlawaysFailJob>(new RetryJobRegisterInfo{ UsedRuleName= customRuleName}),
                retry.RegisterJob<MaybeFailJob>(new RetryJobRegisterInfo{ JobMap=new Dictionary<string,object>{ {"Id",1 } } , UsedRuleName= customRuleName}),
                retry.RegisterJob<MaybeFailJob>(new RetryJobRegisterInfo{ JobMap=new Dictionary<string,object>{ {"Id",2 } } ,UsedRuleName= customRuleName}),
                retry.RegisterJob<MaybeFailJob>(new RetryJobRegisterInfo{ JobMap=new Dictionary<string,object>{ {"Id",3 } } ,UsedRuleName= customRuleName}),
                retry.RegisterJob<MaybeFailJob>(new RetryJobRegisterInfo{ JobMap=new Dictionary<string,object>{ {"Id",4 } } ,UsedRuleName= customRuleName}),
                retry.RegisterJob<MaybeFailJob>(new RetryJobRegisterInfo{ JobMap=new Dictionary<string,object>{ {"Id",5 } } ,UsedRuleName= customRuleName}),
            };

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
