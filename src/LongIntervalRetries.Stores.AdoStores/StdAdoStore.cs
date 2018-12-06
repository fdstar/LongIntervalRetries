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

using Dapper;
using LongIntervalRetries.Stores.AdoStores.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LongIntervalRetries.Stores.AdoStores
{
    /// <summary>
    /// 数据库存储基础实现
    /// </summary>
    public abstract class StdAdoStore : IStore<long>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbFunc">用于获取IDbConnection的委托，注意该IDbConnection每次使用后都会被释放</param>
        /// <param name="tablePrefix">表名前缀，默认不包含前缀，注意下划线(_)不属于前缀</param>
        public StdAdoStore(Func<IDbConnection> dbFunc, string tablePrefix = "")
        {
            this.DbFunc = dbFunc ?? throw new ArgumentNullException();
            this.TablePrefix = tablePrefix ?? string.Empty;
        }
        /// <summary>
        /// 表名前缀
        /// </summary>
        protected string TablePrefix { get; private set; }
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        protected Func<IDbConnection> DbFunc { get; private set; }
        /// <summary>
        /// 获取所有未完成记录的sql
        /// </summary>
        protected abstract string GetAllUnfinishedSqlWithRetryStore { get; }
        /// <summary>
        /// 获取所有未完成记录数据的sql
        /// </summary>
        protected abstract string GetAllUnfinishedSqlWithRetryStoreData { get; }
        /// <summary>
        /// 新增并且返回重试记录自增Id的Sql
        /// </summary>
        protected abstract string InsertAndGetIdSqlWithRetryStore { get; }
        /// <summary>
        /// 新增重试记录数据的Sql
        /// </summary>
        protected abstract string InsertSqlWithRetryStoreData { get; }
        /// <summary>
        /// 更新执行信息Sql
        /// </summary>
        protected abstract string ExecutedSqlWithRetryStore { get; }
        /// <summary>
        /// 更新执行信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task Executed(StoredExecutedInfo<long> entity)
        {
            await this.ExecuteAsync(async conn =>
            {
                return await conn.ExecuteAsync(this.ExecutedSqlWithRetryStore, new RetryStore
                {
                    Id = entity.Id,
                    ExecutedNumber = entity.ExecutedNumber,
                    JobStatus = entity.JobStatus,
                    LastModificationTime = DateTime.Now,
                    PreviousFireTime = entity.PreviousFireTimeUtc.HasValue ? entity.PreviousFireTimeUtc.Value.LocalDateTime : DateTime.Now,
                }).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }
        /// <summary>
        /// 获取所有尚未结束需重试执行的任务
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<StoredInfo<long>>> GetAllUnfinishedRetries()
        {
            return await this.ExecuteAsync(async conn =>
            {
                IEnumerable<StoredInfo<long>> list = null;
                var stores = await conn.QueryAsync<RetryStore>(this.GetAllUnfinishedSqlWithRetryStore, commandTimeout: 60).ConfigureAwait(false);
                if (stores != null && stores.Any())
                {
                    var datas = await conn.QueryAsync<RetryStoreData>(this.GetAllUnfinishedSqlWithRetryStoreData, commandTimeout: 120).ConfigureAwait(false) ?? new List<RetryStoreData>();
                    list = from s in stores
                           join rds in datas on s.Id equals rds.RetryStoreId into temp
                           from d in temp.DefaultIfEmpty()
                           group d by s into g
                           select new StoredInfo<long>
                           {
                               Id = g.Key.Id,
                               ExecutedNumber = g.Key.ExecutedNumber,
                               JobStatus = g.Key.JobStatus,
                               PreviousFireTimeUtc = new DateTimeOffset(g.Key.PreviousFireTime),
                               DeathTimeUtc = new DateTimeOffset(g.Key.DeathTime),
                               UsedRuleName = g.Key.UsedRuleName,
                               JobType = this.GetType(g.Key.JobTypeName),
                               JobMap = Deserialize(g)
                           };
                }
                return list;
            }).ConfigureAwait(false);
        }
        /// <summary>
        /// 根据存储的typeName获取对应的Type
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        protected virtual Type GetType(string typeName)
        {
            return Type.GetType(typeName);
        }
        /// <summary>
        /// 获取Type对应的完整类名，默认包含程序集名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual string GetTypeName(Type type)
        {
            return string.Format("{0},{1}", type.FullName, type.Assembly.GetName().Name);
        }
        /// <summary>
        /// 将<see cref="StoredInfo{TKey}.JobMap"/>序列化为数据集合
        /// </summary>
        /// <param name="retryStoreId"><see cref="RetryStore.Id"/></param>
        /// <param name="jobMap"><see cref="StoredInfo{TKey}.JobMap"/></param>
        /// <returns></returns>
        protected virtual IEnumerable<RetryStoreData> Serializer(long retryStoreId, IDictionary<string, object> jobMap)
        {
            if (jobMap != null && jobMap.Count > 0)
            {
                return jobMap.Where(kv => kv.Value != null).Select(kv => new RetryStoreData
                {
                    RetryStoreId = retryStoreId,
                    KeyName = kv.Key,
                    DataTypeName = this.GetTypeName(kv.Value.GetType()),
                    DataContent = JsonConvert.SerializeObject(kv.Value),
                    CreationTime = DateTime.Now,
                });
            }
            return null;
        }
        /// <summary>
        /// 将<see cref="RetryStoreData"/>反序列化为<see cref="StoredInfo{TKey}.JobMap"/>
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        protected virtual IDictionary<string, object> Deserialize(IEnumerable<RetryStoreData> datas)
        {
            return datas.Where(d => d != null).ToDictionary(k => k.KeyName, v => JsonConvert.DeserializeObject(v.DataContent, this.GetType(v.DataTypeName)));
        }
        /// <summary>
        /// 添加Job信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<long> InsertAndGetId(StoredInfo<long> entity)
        {
            return await this.ExecuteAsync(async conn =>
            {
                var store = new RetryStore
                {
                    CreationTime = DateTime.Now,
                    ExecutedNumber = entity.ExecutedNumber,
                    JobStatus = entity.JobStatus,
                    JobTypeName = this.GetTypeName(entity.JobType),
                    LastModificationTime = DateTime.Now,
                    PreviousFireTime = entity.PreviousFireTimeUtc.HasValue ? entity.PreviousFireTimeUtc.Value.LocalDateTime : DateTime.Now,
                    DeathTime = entity.DeathTimeUtc.HasValue ? entity.DeathTimeUtc.Value.LocalDateTime : DateTime.MaxValue,
                    UsedRuleName = entity.UsedRuleName ?? string.Empty,
                };
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var retryId = await conn.ExecuteScalarAsync<long>(this.InsertAndGetIdSqlWithRetryStore, store, trans).ConfigureAwait(false);
                        if (retryId > 0)
                        {
                            var datas = this.Serializer(retryId, entity.JobMap);
                            if (datas != null && datas.Any())
                            {
                                if (await conn.ExecuteAsync(this.InsertSqlWithRetryStoreData, datas, trans).ConfigureAwait(false) == 0)
                                {
                                    throw new Exception("Insert Fail");
                                }
                            }
                            trans.Commit();
                            return retryId;
                        }
                        throw new Exception("Insert Fail");
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }).ConfigureAwait(false);
        }
        /// <summary>
        /// 执行Sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exeFunc"></param>
        /// <returns></returns>
        protected async Task<T> ExecuteAsync<T>(Func<IDbConnection, Task<T>> exeFunc)
        {
            using (var conn = DbFunc())
            {
                conn.Open();
                return await exeFunc(conn).ConfigureAwait(false);
            }
        }
    }
}
