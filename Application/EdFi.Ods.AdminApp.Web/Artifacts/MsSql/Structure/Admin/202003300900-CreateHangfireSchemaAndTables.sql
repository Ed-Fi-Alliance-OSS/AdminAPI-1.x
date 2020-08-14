-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE SCHEMA [adminapp_HangFire]
GO
CREATE TABLE [adminapp_HangFire].[AggregatedCounter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Value] [bigint] NOT NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_CounterAggregated] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]
GO
CREATE TABLE [adminapp_HangFire].[Counter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Value] [smallint] NOT NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_Counter] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]
GO
CREATE TABLE [adminapp_HangFire].[Hash](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Field] [nvarchar](100) NOT NULL,
	[Value] [nvarchar](max) NULL,
	[ExpireAt] [datetime2](7) NULL,
 CONSTRAINT [PK_HangFire_Hash] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
CREATE TABLE [adminapp_HangFire].[Job](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StateId] [int] NULL,
	[StateName] [nvarchar](20) NULL,
	[InvocationData] [nvarchar](max) NOT NULL,
	[Arguments] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_Job] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
CREATE TABLE [adminapp_HangFire].[JobParameter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JobId] [int] NOT NULL,
	[Name] [nvarchar](40) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_HangFire_JobParameter] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
CREATE TABLE [adminapp_HangFire].[JobQueue](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JobId] [int] NOT NULL,
	[Queue] [nvarchar](50) NOT NULL,
	[FetchedAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_JobQueue] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]
GO
CREATE TABLE [adminapp_HangFire].[List](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Value] [nvarchar](max) NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_List] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
CREATE TABLE [adminapp_HangFire].[Schema](
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_HangFire_Schema] PRIMARY KEY CLUSTERED 
(
	[Version] ASC
)
) ON [PRIMARY]
GO
CREATE TABLE [adminapp_HangFire].[Server](
	[Id] [nvarchar](100) NOT NULL,
	[Data] [nvarchar](max) NULL,
	[LastHeartbeat] [datetime] NOT NULL,
 CONSTRAINT [PK_HangFire_Server] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
CREATE TABLE [adminapp_HangFire].[Set](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Score] [float] NOT NULL,
	[Value] [nvarchar](256) NOT NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_Set] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]
GO
CREATE TABLE [adminapp_HangFire].[State](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JobId] [int] NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
	[Reason] [nvarchar](100) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[Data] [nvarchar](max) NULL,
 CONSTRAINT [PK_HangFire_State] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_HangFire_CounterAggregated_Key] ON [adminapp_HangFire].[AggregatedCounter]
(
	[Key] ASC
)
INCLUDE ( 	[Value]) 
GO
CREATE NONCLUSTERED INDEX [IX_HangFire_Counter_Key] ON [adminapp_HangFire].[Counter]
(
	[Key] ASC
)
INCLUDE ( 	[Value]) 
GO
CREATE NONCLUSTERED INDEX [IX_HangFire_Hash_ExpireAt] ON [adminapp_HangFire].[Hash]
(
	[ExpireAt] ASC
)
INCLUDE ( 	[Id]) 
GO
CREATE NONCLUSTERED INDEX [IX_HangFire_Hash_Key] ON [adminapp_HangFire].[Hash]
(
	[Key] ASC
)
INCLUDE ( 	[ExpireAt]) 
GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_HangFire_Hash_Key_Field] ON [adminapp_HangFire].[Hash]
(
	[Key] ASC,
	[Field] ASC
)
GO
CREATE NONCLUSTERED INDEX [IX_HangFire_Job_ExpireAt] ON [adminapp_HangFire].[Job]
(
	[ExpireAt] ASC
)
INCLUDE ( 	[Id]) 
GO
CREATE NONCLUSTERED INDEX [IX_HangFire_Job_StateName] ON [adminapp_HangFire].[Job]
(
	[StateName] ASC
)
GO
CREATE NONCLUSTERED INDEX [IX_HangFire_JobParameter_JobIdAndName] ON [adminapp_HangFire].[JobParameter]
(
	[JobId] ASC,
	[Name] ASC
)
GO
CREATE NONCLUSTERED INDEX [IX_HangFire_JobQueue_QueueAndFetchedAt] ON [adminapp_HangFire].[JobQueue]
(
	[Queue] ASC,
	[FetchedAt] ASC
)
GO
CREATE NONCLUSTERED INDEX [IX_HangFire_List_ExpireAt] ON [adminapp_HangFire].[List]
(
	[ExpireAt] ASC
)
INCLUDE ( 	[Id]) 
GO
CREATE NONCLUSTERED INDEX [IX_HangFire_List_Key] ON [adminapp_HangFire].[List]
(
	[Key] ASC
)
INCLUDE ( 	[ExpireAt],
	[Value]) 
GO
CREATE NONCLUSTERED INDEX [IX_HangFire_Set_ExpireAt] ON [adminapp_HangFire].[Set]
(
	[ExpireAt] ASC
)
INCLUDE ( 	[Id]) 
GO
CREATE NONCLUSTERED INDEX [IX_HangFire_Set_Key] ON [adminapp_HangFire].[Set]
(
	[Key] ASC
)
INCLUDE ( 	[ExpireAt],
	[Value]) 
GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_HangFire_Set_KeyAndValue] ON [adminapp_HangFire].[Set]
(
	[Key] ASC,
	[Value] ASC
)
GO
CREATE NONCLUSTERED INDEX [IX_HangFire_State_JobId] ON [adminapp_HangFire].[State]
(
	[JobId] ASC
)
GO
ALTER TABLE [adminapp_HangFire].[JobParameter]  WITH CHECK ADD  CONSTRAINT [FK_HangFire_JobParameter_Job] FOREIGN KEY([JobId])
REFERENCES [adminapp_HangFire].[Job] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [adminapp_HangFire].[JobParameter] CHECK CONSTRAINT [FK_HangFire_JobParameter_Job]
GO
ALTER TABLE [adminapp_HangFire].[State]  WITH CHECK ADD  CONSTRAINT [FK_HangFire_State_Job] FOREIGN KEY([JobId])
REFERENCES [adminapp_HangFire].[Job] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [adminapp_HangFire].[State] CHECK CONSTRAINT [FK_HangFire_State_Job]
GO