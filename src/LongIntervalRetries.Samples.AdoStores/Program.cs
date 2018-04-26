using LongIntervalRetries.Rules;
using LongIntervalRetries.Samples.Jobs;
using LongIntervalRetries.Stores.AdoStores;
using MySql.Data.MySqlClient;
using System;

namespace LongIntervalRetries.Samples.AdoStores
{
    class Program
    {
        static void Main(string[] args)
        {
            Demo();
            Console.ReadLine();
        }

        static async void Demo()
        {
            var connectionString = "Server=127.0.0.1;Port=3306;Database=test; User=root;Password=123456;Connection Timeout=600;Charset=utf8";
            var retry = new StdRetry(store: new MySQLStore(() => new MySqlConnection(connectionString)));
            string simpleRuleName = "SimpleRepeatRetryRule";
            var simpleRepeatRule = new SimpleRepeatRetryRule(simpleRuleName, 50, TimeSpan.FromSeconds(5));
            retry.RuleManager.AddRule(simpleRepeatRule);
            await retry.RegisterJob<AlawaysFailJob>(new RetryJobRegisterInfo()).ConfigureAwait(false);
            retry.Start();
        }
    }
}
