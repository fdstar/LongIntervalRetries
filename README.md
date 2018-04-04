# LongIntervalRetries
这是一个基于Quartz.Net的重试类库，该类库对应需长时间间隔运行的业务类型，比如通知服务

## .NET版本支持
目前支持以下版本：`.NET452`、`.NET Standard 2.0`

## 使用说明
`StdRetry<T>`为类库入口，其构造函数具备以下参数   
* `IScheduler scheduler = null`，即`Quartz.IScheduler`，如不传入，则采用默认`Quartz.Impl.StdSchedulerFactory.GetDefaultScheduler()`,注意配置文件`quartz.config`在默认实现时依然有效
* `IStore<T> store = null`，定义重试信息如何存储与恢复，如不传入，则默认采用`LongIntervalRetries.Stores.NoneStore`，即不采取任何存储
* `IRetryRuleManager ruleManager = null`，定义重试策略`IRetryRule`的管理类，如不传入，则默认采用`LongIntervalRetries.Rules.StdRetryRuleManager`，注意无特殊情况无需自己实现接口定义
* `IRetryJobListener retryJobListener = null`，定义Job的重试监控策略，如不传入，则默认采用`LongIntervalRetries.StdRetryJobListener`，该实现默认按`Exception`来判断是否需要重试，注意无特殊情况无需自己实现接口定义

### `IScheduler`
具体说明请参考Quartz.Net

### `IStore<T>`
后续提供相关Sql存储实现

### `IRetryRuleManager`
`IRetryRule`默认提供以下两种实现
* `SimpleRepeatRetryRule`简单重试规则，该规则简单的定义了最大重试次数以及简单的重试间隔
* `CustomIntervalRetryRule`自定义间隔重试规则，该规则接收一系列的时间参数`params TimeSpan[] intervals`，该数组的`Length`属性即为允许尝试的最大次数，该规则会直接将执行次数作为索引获取对应`TimeSpan`

### 快速使用
```csharp
var retry = new StdRetry();
//声明并注册重试规则
string simpleRuleName = "SimpleRepeatRetryRule";
var simpleRepeatRule = new SimpleRepeatRetryRule(simpleRuleName, 5, TimeSpan.FromSeconds(2));
retry.RuleManager.AddRule(simpleRepeatRule);
var registerInfo = new
{
    //指定要采用的重试规则，如果不设置，则默认使用已注册的第一项
    UsedRuleName = simpleRuleName,
    //需要传递给IJob的上下文数据
    JobMap = new Dictionary<string, object>
    {
        {"SomeKey","SomeValue" }
    },
    //开始执行时间，如果不指定则表示立刻执行
    StartAt = DateTimeOffset.UtcNow.AddSeconds(3),
};
//注册要执行的Job
retry.RegisterJob<SomeJob>(registerInfo);
//注册每次Job执行后的通知事件
retry.RegisterEvent<SomeJob>(e =>
{//Some code
});
retry.Start();//启动Quartz服务
//启动服务后仍可以RegisterJob、RegisterEvent
```

## 完整的例子
### LongIntervalRetries.Samples.Jobs
该例子包含以下Job
* `SimpleJob`所有例子Job的基类
* `AlawaysSuccessJob`永远执行成功的Job例子
* `AlawaysFailJob`永远执行失败的Job例子，该例子可用于测试重试机制
* `MaybeFailJob`可能会执行失败的Job例子，该例子最符合实际情况，根据随机数来决定是否会抛出异常

### LongIntervalRetries.Samples.Default
简单但又完整的使用例子，包含了如何注册`IRetryRule`，如何注册`IJob`，如何注册执行结果事件，如何通过`RetryJobRegisterInfo.JobMap`来传递参数到实际执行的`IJob`
