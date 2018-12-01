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

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LongIntervalRetries.Stores.AdoStores
{
    /// <summary>
    /// MySQL存储实现
    /// </summary>
    public class MySQLStore : StdAdoStore
    {
        /// <summary>
        /// MySQL存储实现
        /// </summary>
        /// <param name="dbFunc"></param>
        /// <param name="tablePrefix"></param>
        public MySQLStore(Func<IDbConnection> dbFunc, string tablePrefix = "")
            : base(dbFunc, tablePrefix)
        {
        }
        /// <summary>
        /// 获取所有未完成记录的sql
        /// </summary>
        protected override string GetAllUnfinishedSqlWithRetryStore
        {
            get
            {
                return $@"SELECT `Id`,`JobTypeName`,`ExecutedNumber`,`PreviousFireTime`,`DeathTime`,`UsedRuleName`,`JobStatus`,`CreationTime`,`LastModificationTime`
FROM `{this.TablePrefix}_RetryStores` WHERE `JobStatus` = 0";
            }
        }
        /// <summary>
        /// 获取所有未完成记录数据的sql
        /// </summary>
        protected override string GetAllUnfinishedSqlWithRetryStoreData
        {
            get
            {
                return $@"SELECT `{this.TablePrefix}_RetryStoreDatas`.`Id`,`RetryStoreId`,`KeyName`,`DataContent`,`DataTypeName`,`{this.TablePrefix}_RetryStoreDatas`.`CreationTime`
FROM `{this.TablePrefix}_RetryStores` JOIN `{this.TablePrefix}_RetryStoreDatas` ON `{this.TablePrefix}_RetryStoreDatas`.`RetryStoreId` = `{this.TablePrefix}_RetryStores`.`Id`
WHERE `{this.TablePrefix}_RetryStores`.`JobStatus` = 0";
            }
        }
        /// <summary>
        /// 新增并且返回重试记录自增Id的Sql
        /// </summary>
        protected override string InsertAndGetIdSqlWithRetryStore
        {
            get
            {
                return $@"INSERT INTO `{this.TablePrefix}_RetryStores` 
(`JobTypeName`,`ExecutedNumber`,`PreviousFireTime`,`DeathTime`,`UsedRuleName`,`JobStatus`,`CreationTime`,`LastModificationTime`) 
VALUES
(@JobTypeName,@ExecutedNumber,@PreviousFireTime,@DeathTime,@UsedRuleName,@JobStatus,@CreationTime,@LastModificationTime);
SELECT CONVERT(LAST_INSERT_ID(), unsigned integer) AS ID";
            }
        }
        /// <summary>
        /// 新增重试记录数据的Sql
        /// </summary>
        protected override string InsertSqlWithRetryStoreData
        {
            get
            {
                return $@"INSERT INTO `{this.TablePrefix}_RetryStoreDatas` 
(`RetryStoreId`,`KeyName`,`DataContent`,`DataTypeName`,`CreationTime`) 
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
                return $@"UPDATE `{this.TablePrefix}_RetryStores`
SET `ExecutedNumber`=@ExecutedNumber,`PreviousFireTime`=@PreviousFireTime,`JobStatus`= @JobStatus,`LastModificationTime`= @LastModificationTime
WHERE `Id`=@Id";
            }
        }
    }
}
