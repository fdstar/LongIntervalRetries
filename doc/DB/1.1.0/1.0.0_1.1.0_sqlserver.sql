alter table _RetryStores add [DeathTime] datetime not null default '9999-12-31';
alter table _RetryStores add [LastException] varchar(max) null;