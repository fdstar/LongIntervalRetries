using LongIntervalRetries.Rules;
using LongIntervalRetries.Samples.Jobs;
using LongIntervalRetries.Stores;
using LongIntervalRetries.Stores.AdoStores;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace LongIntervalRetries.Samples.AdoStores
{
    class Program
    {
        protected static readonly Logger Logger = LogManager.GetLogger("Default");
        static void Main(string[] args)
        {
            AdoStoreDemo();
            Console.ReadLine();
        }

        static async void AdoStoreDemo()
        {
            var store = GetMySqlStore();
            var retry = new StdRetry(store: store);
            string simpleRuleName = "SimpleRepeatRetryRule";
            var simpleRepeatRule = new SimpleRepeatRetryRule(simpleRuleName, 50, TimeSpan.FromSeconds(5));
            retry.RuleManager.AddRule(simpleRepeatRule);
            retry.RegisterEvent<AlawaysFailJob>(e =>
            {
                Logger.Info("AlawaysFailJob Result:{0}", JsonConvert.SerializeObject(e));
            });
            retry.Start();
            //return;
            for (var i = 0; i < 1; i++)
            {
                SimpleJob.Logger.Info("RegisterJob " + i);
                var info = new RetryJobRegisterInfo
                {
                    JobMap = new Dictionary<string, object>
                    {
                        { "Id",i},
                        { "key2","stringKey"+i},
                    }
                };
                await retry.RegisterJob<AlawaysFailJob>(info).ConfigureAwait(false);
            }
        }
        const string tablePrefix = "";
        static IStore<long> GetMySqlStore()
        {
            var connectionString = "Server=127.0.0.1;Port=3306;Database=test; User=root;Password=123456;Connection Timeout=600;Charset=utf8";
            return new MySQLStore(() => new MySqlConnection(connectionString), tablePrefix);
        }
        static IStore<long> GetSqlServerDbFunc()
        {
            var connectionString = "Uid=sa;Pwd=123456;Initial Catalog=Test;Data Source=.;Connect Timeout=900";
            return new SqlServerStore(() => new SqlConnection(connectionString), tablePrefix: tablePrefix);
        }
    }
}
