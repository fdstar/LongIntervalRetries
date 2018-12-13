alter table `_RetryStores` add column `DeathTime` datetime not null default '9999-12-31' COMMENT 'Job死亡时间';
alter table `_RetryStores` add column `LastException` LONGTEXT null;