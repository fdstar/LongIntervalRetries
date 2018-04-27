using LongIntervalRetries.Rules;
using LongIntervalRetries.Samples.Jobs;
using LongIntervalRetries.Stores;
using LongIntervalRetries.Stores.AdoStores;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

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
            var store = GetSqlServerDbFunc();
            var retry = new StdRetry(store: store);
            string simpleRuleName = "SimpleRepeatRetryRule";
            var simpleRepeatRule = new SimpleRepeatRetryRule(simpleRuleName, 50, TimeSpan.FromSeconds(5));
            retry.RuleManager.AddRule(simpleRepeatRule);
            var jobMap = new Dictionary<string, object>
            {
                { "key1",1},
                { "key2","stringKey"}
            };
            //可以注销RegisterJob来测试从数据库恢复Job的功能
            await retry.RegisterJob<AlawaysFailJob>(new RetryJobRegisterInfo
            {
                JobMap = jobMap
            }).ConfigureAwait(false);
            retry.Start();
        }
        static IStore<long> GetMySqlStore()
        {
            var connectionString = "Server=127.0.0.1;Port=3306;Database=test; User=root;Password=123456;Connection Timeout=600;Charset=utf8";
            return new MySQLStore(() => new MySqlConnection(connectionString));
        }
        static IStore<long> GetSqlServerDbFunc()
        {
            var connectionString = "Uid=sa;Pwd=123456;Initial Catalog=Test;Data Source=.;Connect Timeout=900";
            return new SqlServerStore(() => new SqlConnection(connectionString));
        }
    }
}
