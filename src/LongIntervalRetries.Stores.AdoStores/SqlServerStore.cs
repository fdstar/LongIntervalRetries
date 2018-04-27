﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LongIntervalRetries.Stores.AdoStores
{
    /// <summary>
    /// MsSql存储实现
    /// </summary>
    public class SqlServerStore : StdAdoStore
    {
        private string _owner;
        /// <summary>
        /// MsSql存储实现
        /// </summary>
        /// <param name="dbFunc"></param>
        /// <param name="owner">数据库拥有者</param>
        public SqlServerStore(Func<IDbConnection> dbFunc, string owner = "dbo")
            : base(dbFunc)
        {
            this._owner = string.IsNullOrWhiteSpace(owner) ? "" : string.Format("{0}.", owner);
        }
        /// <summary>
        /// 获取所有未完成记录的sql
        /// </summary>
        protected override string GetAllUnfinishedSqlWithRetryStore
        {
            get
            {
                return $@"SELECT [Id],[JobTypeName],[ExecutedNumber],[PreviousFireTime],[UsedRuleName],[JobStatus],[CreationTime],[LastModificationTime]
FROM {this._owner}_RetryStores WHERE [JobStatus] = 0";
            }
        }
        /// <summary>
        /// 获取所有未完成记录数据的sql
        /// </summary>
        protected override string GetAllUnfinishedSqlWithRetryStoreData
        {
            get
            {
                return $@"SELECT rsd.[Id],[RetryStoreId],[KeyName],[DataContent],[DataTypeName],rsd.[CreationTime]
FROM {this._owner}_RetryStores rs JOIN {this._owner}_RetryStoreDatas rsd ON rsd.[RetryStoreId] = rs.[Id]
WHERE rs.[JobStatus] = 0";
            }
        }
        /// <summary>
        /// 新增并且返回重试记录自增Id的Sql
        /// </summary>
        protected override string InsertAndGetIdSqlWithRetryStore
        {
            get
            {
                return $@"INSERT INTO {this._owner}_RetryStores 
([JobTypeName],[ExecutedNumber],[PreviousFireTime],[UsedRuleName],[JobStatus],[CreationTime],[LastModificationTime]) 
VALUES
(@JobTypeName,@ExecutedNumber,@PreviousFireTime,@UsedRuleName,@JobStatus,@CreationTime,@LastModificationTime);
SELECT @@IDENTITY";
            }
        }
        /// <summary>
        /// 新增重试记录数据的Sql
        /// </summary>
        protected override string InsertSqlWithRetryStoreData
        {
            get
            {
                return $@"INSERT INTO {this._owner}_RetryStoreDatas 
([RetryStoreId],[KeyName],[DataContent],[DataTypeName],[CreationTime]) 
VALUES
(@RetryStoreId,@KeyName,@DataContent,@DataTypeName,@CreationTime)";
            }
        }
        /// <summary>
        /// 更新执行信息Sql
        /// </summary>
        protected override string ExecutedSqlWithRetryStore
        {
            get
            {
                return $@"UPDATE {this._owner}_RetryStores
SET [ExecutedNumber]=@ExecutedNumber,[PreviousFireTime]=@PreviousFireTime,[JobStatus]= @JobStatus,[LastModificationTime]= @LastModificationTime
WHERE [Id]=@Id";
            }
        }
    }
}
