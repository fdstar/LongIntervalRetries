CREATE TABLE `_RetryStores` (
	`Id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '自增主键',
	`JobTypeName` VARCHAR(200) NOT NULL COMMENT '完整的Job全名，包含程序集名称',
	`ExecutedNumber` INT(11) NOT NULL DEFAULT 0  COMMENT '当前已执行次数',
	`PreviousFireTime` DATETIME NOT NULL COMMENT '上一次执行时间',
	`UsedRuleName` VARCHAR(100) NOT NULL COMMENT '要采用的规则' DEFAULT '',
	`JobStatus` TINYINT(4) NOT NULL DEFAULT 0 COMMENT '当前Job状态',
	`CreationTime` Timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
	`LastModificationTime` Timestamp NOT NULL COMMENT '最后修改时间' DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
	PRIMARY KEY (`Id`),
	INDEX `IX_RetryStores_JobStatus` (`JobStatus`)
)
DEFAULT CHARSET=utf8 
COLLATE='utf8_general_ci'
ENGINE=InnoDB;

CREATE TABLE `_RetryStoreDatas` (
	`Id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '自增主键',
	`RetryStoreId` BIGINT(20) NOT NULL COMMENT '_RetryStores.Id',
	`KeyName` VARCHAR(200) NOT NULL COMMENT '键值名',
	`DataContent` Text NOT NULL COMMENT '序列化后的数据内容',
	`DataTypeName` VARCHAR(200) NOT NULL COMMENT '完整的值类型全名，包含程序集名称',
	`CreationTime` Timestamp NOT NULL COMMENT '创建时间' DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY (`Id`),
	CONSTRAINT `FK_RetryStores_Id` FOREIGN KEY (`RetryStoreId`) REFERENCES `_RetryStores` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
	INDEX `IX_RetryStoreDatas_RetryStoreId` (`RetryStoreId`)
)
DEFAULT CHARSET=utf8 
COLLATE='utf8_general_ci'
ENGINE=InnoDB;