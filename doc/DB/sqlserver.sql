if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('dbo._RetryStoreDatas') and o.name = 'FK_RetryStores_Id')
alter table dbo._RetryStoreDatas
   drop constraint FK_RetryStores_Id
go

if exists (select 1
            from  sysobjects
           where  id = object_id('dbo._RetryStoreDatas')
            and   type = 'U')
   drop table dbo._RetryStoreDatas
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('dbo._RetryStores')
            and   name  = 'IX_RetryStores_JobStatus'
            and   indid > 0
            and   indid < 255)
   drop index dbo._RetryStores.IX_RetryStores_JobStatus
go

if exists (select 1
            from  sysobjects
           where  id = object_id('dbo._RetryStores')
            and   type = 'U')
   drop table dbo._RetryStores
go

/*==============================================================*/
/* Table: _RetryStoreDatas                                      */
/*==============================================================*/
create table dbo._RetryStoreDatas (
   Id                   bigint               identity(1,1) not for replication,
   RetryStoreId         bigint               not null,
   KeyName              varchar(200)         not null,
   DataContent          varchar(Max)         not null,
   DataTypeName         varchar(200)         not null,
   CreationTime         datetime             not null default getdate(),
   constraint PK__RetryStoreDatas primary key (Id)
)
go

if exists (select 1 from  sys.extended_properties
           where major_id = object_id('dbo._RetryStoreDatas') and minor_id = 0)
begin 
   execute sp_dropextendedproperty 'MS_Description',  
   'user', 'dbo', 'table', '_RetryStoreDatas' 
 
end 


execute sp_addextendedproperty 'MS_Description',  
   '重试相关的上下文数据信息', 
   'user', 'dbo', 'table', '_RetryStoreDatas'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('dbo._RetryStoreDatas')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Id')
)
begin
   execute sp_dropextendedproperty 'MS_Description', 
   'user', 'dbo', 'table', '_RetryStoreDatas', 'column', 'Id'

end


execute sp_addextendedproperty 'MS_Description', 
   '自增主键',
   'user', 'dbo', 'table', '_RetryStoreDatas', 'column', 'Id'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('dbo._RetryStoreDatas')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'RetryStoreId')
)
begin
   execute sp_dropextendedproperty 'MS_Description', 
   'user', 'dbo', 'table', '_RetryStoreDatas', 'column', 'RetryStoreId'

end


execute sp_addextendedproperty 'MS_Description', 
   '_RetryStores.Id',
   'user', 'dbo', 'table', '_RetryStoreDatas', 'column', 'RetryStoreId'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('dbo._RetryStoreDatas')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'KeyName')
)
begin
   execute sp_dropextendedproperty 'MS_Description', 
   'user', 'dbo', 'table', '_RetryStoreDatas', 'column', 'KeyName'

end


execute sp_addextendedproperty 'MS_Description', 
   '键值名',
   'user', 'dbo', 'table', '_RetryStoreDatas', 'column', 'KeyName'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('dbo._RetryStoreDatas')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'DataContent')
)
begin
   execute sp_dropextendedproperty 'MS_Description', 
   'user', 'dbo', 'table', '_RetryStoreDatas', 'column', 'DataContent'

end


execute sp_addextendedproperty 'MS_Description', 
   '序列化后的数据内容',
   'user', 'dbo', 'table', '_RetryStoreDatas', 'column', 'DataContent'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('dbo._RetryStoreDatas')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'DataTypeName')
)
begin
   execute sp_dropextendedproperty 'MS_Description', 
   'user', 'dbo', 'table', '_RetryStoreDatas', 'column', 'DataTypeName'

end


execute sp_addextendedproperty 'MS_Description', 
   '完整的值类型全名，包含程序集名称',
   'user', 'dbo', 'table', '_RetryStoreDatas', 'column', 'DataTypeName'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('dbo._RetryStoreDatas')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'CreationTime')
)
begin
   execute sp_dropextendedproperty 'MS_Description', 
   'user', 'dbo', 'table', '_RetryStoreDatas', 'column', 'CreationTime'

end


execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', 'dbo', 'table', '_RetryStoreDatas', 'column', 'CreationTime'
go

/*==============================================================*/
/* Table: _RetryStores                                          */
/*==============================================================*/
create table dbo._RetryStores (
   Id                   bigint               identity(1,1) not for replication,
   JobTypeName          varchar(200)         not null,
   ExecutedNumber       int                  not null default 0,
   PreviousFireTime     datetime             not null default getdate(),
   UsedRuleName         varchar(100)         not null,
   JobStatus            tinyint              not null,
   CreationTime         datetime             not null default getdate(),
   LastModificationTime datetime             not null default getdate(),
   constraint PK__RetryStores primary key nonclustered (Id)
)
go

if exists (select 1 from  sys.extended_properties
           where major_id = object_id('dbo._RetryStores') and minor_id = 0)
begin 
   execute sp_dropextendedproperty 'MS_Description',  
   'user', 'dbo', 'table', '_RetryStores' 
 
end 


execute sp_addextendedproperty 'MS_Description',  
   '重试存储信息', 
   'user', 'dbo', 'table', '_RetryStores'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('dbo._RetryStores')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'Id')
)
begin
   execute sp_dropextendedproperty 'MS_Description', 
   'user', 'dbo', 'table', '_RetryStores', 'column', 'Id'

end


execute sp_addextendedproperty 'MS_Description', 
   '自增主键',
   'user', 'dbo', 'table', '_RetryStores', 'column', 'Id'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('dbo._RetryStores')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'JobTypeName')
)
begin
   execute sp_dropextendedproperty 'MS_Description', 
   'user', 'dbo', 'table', '_RetryStores', 'column', 'JobTypeName'

end


execute sp_addextendedproperty 'MS_Description', 
   '完整的Job全名，包含程序集名称',
   'user', 'dbo', 'table', '_RetryStores', 'column', 'JobTypeName'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('dbo._RetryStores')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'ExecutedNumber')
)
begin
   execute sp_dropextendedproperty 'MS_Description', 
   'user', 'dbo', 'table', '_RetryStores', 'column', 'ExecutedNumber'

end


execute sp_addextendedproperty 'MS_Description', 
   '当前已执行次数',
   'user', 'dbo', 'table', '_RetryStores', 'column', 'ExecutedNumber'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('dbo._RetryStores')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'PreviousFireTime')
)
begin
   execute sp_dropextendedproperty 'MS_Description', 
   'user', 'dbo', 'table', '_RetryStores', 'column', 'PreviousFireTime'

end


execute sp_addextendedproperty 'MS_Description', 
   '上一次执行时间',
   'user', 'dbo', 'table', '_RetryStores', 'column', 'PreviousFireTime'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('dbo._RetryStores')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'UsedRuleName')
)
begin
   execute sp_dropextendedproperty 'MS_Description', 
   'user', 'dbo', 'table', '_RetryStores', 'column', 'UsedRuleName'

end


execute sp_addextendedproperty 'MS_Description', 
   '要采用的规则',
   'user', 'dbo', 'table', '_RetryStores', 'column', 'UsedRuleName'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('dbo._RetryStores')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'JobStatus')
)
begin
   execute sp_dropextendedproperty 'MS_Description', 
   'user', 'dbo', 'table', '_RetryStores', 'column', 'JobStatus'

end


execute sp_addextendedproperty 'MS_Description', 
   '当前Job状态',
   'user', 'dbo', 'table', '_RetryStores', 'column', 'JobStatus'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('dbo._RetryStores')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'CreationTime')
)
begin
   execute sp_dropextendedproperty 'MS_Description', 
   'user', 'dbo', 'table', '_RetryStores', 'column', 'CreationTime'

end


execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', 'dbo', 'table', '_RetryStores', 'column', 'CreationTime'
go

if exists(select 1 from sys.extended_properties p where
      p.major_id = object_id('dbo._RetryStores')
  and p.minor_id = (select c.column_id from sys.columns c where c.object_id = p.major_id and c.name = 'LastModificationTime')
)
begin
   execute sp_dropextendedproperty 'MS_Description', 
   'user', 'dbo', 'table', '_RetryStores', 'column', 'LastModificationTime'

end


execute sp_addextendedproperty 'MS_Description', 
   '最后修改时间',
   'user', 'dbo', 'table', '_RetryStores', 'column', 'LastModificationTime'
go

/*==============================================================*/
/* Index: IX_RetryStores_JobStatus                              */
/*==============================================================*/
create clustered index IX_RetryStores_JobStatus on dbo._RetryStores (
JobStatus ASC
)
go

alter table dbo._RetryStoreDatas
   add constraint FK_RetryStores_Id foreign key (RetryStoreId)
      references dbo._RetryStores (Id)
         on update cascade on delete cascade
go
