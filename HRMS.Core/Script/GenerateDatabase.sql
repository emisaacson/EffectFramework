/****** Object:  Table [dbo].[DataTypes]    Script Date: 4/30/2015 2:45:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DataTypes](
	[DataTypeID] [int] NOT NULL,
	[Name] [varchar](1024) NOT NULL,
 CONSTRAINT [PK_DataTypes] PRIMARY KEY CLUSTERED 
(
	[DataTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EmployeeEntities]    Script Date: 4/30/2015 2:45:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeEntities](
	[EmployeeEntityID] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeRecordID] [int] NOT NULL,
	[EntityID] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_EmployeeEntities] PRIMARY KEY CLUSTERED 
(
	[EmployeeEntityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_EmployeeEntities] UNIQUE NONCLUSTERED 
(
	[EmployeeEntityID] ASC,
	[EmployeeRecordID] ASC,
	[EntityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EmployeeRecords]    Script Date: 4/30/2015 2:45:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeRecords](
	[EmployeeRecordID] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeID] [int] NOT NULL,
	[EventID] [int] NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[EndEffectiveDate] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_EmployeeRecords] PRIMARY KEY CLUSTERED 
(
	[EmployeeRecordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Employees]    Script Date: 4/30/2015 2:45:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Employees](
	[EmployeeID] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeRecordID] [int] NULL,
	[IsDeleted] [bit] NOT NULL,
	[DisplayName] [varchar](1024) NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
(
	[EmployeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Entities]    Script Date: 4/30/2015 2:45:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Entities](
	[EntityID] [int] IDENTITY(1,1) NOT NULL,
	[EntityTypeID] [int] NOT NULL,
	[EmployeeID] [int] NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[EndEffectiveDate] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Entities] PRIMARY KEY CLUSTERED 
(
	[EntityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EntityFields]    Script Date: 4/30/2015 2:45:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EntityFields](
	[EntityFieldID] [int] IDENTITY(1,1) NOT NULL,
	[FieldTypeID] [int] NOT NULL,
	[EntityID] [int] NOT NULL,
	[ValueText] [varchar](max) NULL,
	[ValueDate] [datetime] NULL,
	[ValueDecimal] [decimal](18, 4) NULL,
	[ValueBoolean] [bit] NULL,
	[ValueUser] [int] NULL,
	[IsDeleted] [bit] NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_EntityFields] PRIMARY KEY CLUSTERED 
(
	[EntityFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EntityTypes]    Script Date: 4/30/2015 2:45:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EntityTypes](
	[EntityTypeID] [int] NOT NULL,
	[Name] [varchar](1024) NOT NULL,
 CONSTRAINT [PK_EntityTypes] PRIMARY KEY CLUSTERED 
(
	[EntityTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FieldTypes]    Script Date: 4/30/2015 2:45:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FieldTypes](
	[FieldTypeID] [int] NOT NULL,
	[Name] [varchar](1024) NOT NULL,
	[DataTypeID] [int] NOT NULL,
 CONSTRAINT [PK_FieldTypes] PRIMARY KEY CLUSTERED 
(
	[FieldTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[CompleteEmployeeRecord]    Script Date: 4/30/2015 2:45:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[CompleteEmployeeRecord] AS
select
e.EmployeeID,
e.[Guid] as EmployeeGuid,
er.EmployeeRecordID,
er.EventID,
er.EffectiveDate as EmployeeRecordEffectiveDate,
er.EndEffectiveDate as EmployeeRecordEndEffectiveDate,
er.[Guid] as EmployeeRecordGuid,
et.EntityTypeID,
et.Name as EntityTypeName,
ee.EmployeeEntityID,
ee.[Guid] as EmployeeEntityGuid,
en.EntityID,
en.EffectiveDate as EntityEffectiveDate,
en.EndEffectiveDate as EntityEndEffectiveDate,
en.[Guid] as EntityGuid,
ft.FieldTypeID,
ft.Name as FieldTypeName,
dt.DataTypeID,
dt.Name as DataTypeName,
ef.EntityFieldID,
ef.ValueText,
ef.ValueDate,
ef.ValueDecimal,
ef.ValueBoolean,
ef.ValueUser,
ef.[Guid] as EntityFieldGuid
FROM Employees e
JOIN EmployeeRecords er on er.EmployeeID = e.EmployeeID and er.IsDeleted = 0
JOIN EmployeeEntities ee on ee.EmployeeRecordID = er.EmployeeRecordID and ee.IsDeleted = 0
JOIN Entities en on en.EntityID = ee.EntityID and en.IsDeleted = 0
JOIN EntityTypes et on et.EntityTypeID = en.EntityTypeID
JOIN EntityFields ef on ef.EntityID = en.EntityID and ef.IsDeleted = 0
JOIN FieldTypes ft on ft.FieldTypeID = ef.FieldTypeID
JOIN DataTypes dt on dt.DataTypeID = ft.DataTypeID


GO
/****** Object:  View [dbo].[CurrentEmployeeRecord]    Script Date: 4/30/2015 2:45:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[CurrentEmployeeRecord] AS
select
er.EmployeeRecordID,
er.EventID,
er.EffectiveDate as EmployeeRecordEffectiveDate,
er.EndEffectiveDate as EmployeeRecordEndEffectiveDate,
et.EntityTypeID,
et.Name as EntityTypeName,
en.EffectiveDate as EntityEffectiveDate,
en.EndEffectiveDate as EntityEndEffectiveDate,
ft.FieldTypeID,
ft.Name as FieldTypeName,
dt.DataTypeID,
dt.Name as DataTypeName,
ef.ValueText,
ef.ValueDate,
ef.ValueDecimal,
ef.ValueBoolean,
ef.ValueUser
FROM Employees e
JOIN EmployeeRecords er on er.EmployeeRecordID = e.EmployeeRecordID and er.IsDeleted = 0
JOIN EmployeeEntities ee on ee.EmployeeRecordID = er.EmployeeRecordID and ee.IsDeleted = 0
JOIN Entities en on en.EntityID = ee.EntityID and en.IsDeleted = 0
JOIN EntityTypes et on et.EntityTypeID = en.EntityTypeID
JOIN EntityFields ef on ef.EntityID = en.EntityID and ef.IsDeleted = 0
JOIN FieldTypes ft on ft.FieldTypeID = ef.FieldTypeID
JOIN DataTypes dt on dt.DataTypeID = ft.DataTypeID


GO
ALTER TABLE [dbo].[EmployeeEntities]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeEntities_EmployeeRecords] FOREIGN KEY([EmployeeRecordID])
REFERENCES [dbo].[EmployeeRecords] ([EmployeeRecordID])
GO
ALTER TABLE [dbo].[EmployeeEntities] CHECK CONSTRAINT [FK_EmployeeEntities_EmployeeRecords]
GO
ALTER TABLE [dbo].[EmployeeEntities]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeEntities_Entities] FOREIGN KEY([EntityID])
REFERENCES [dbo].[Entities] ([EntityID])
GO
ALTER TABLE [dbo].[EmployeeEntities] CHECK CONSTRAINT [FK_EmployeeEntities_Entities]
GO
ALTER TABLE [dbo].[EmployeeRecords]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeRecords_Employees] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employees] ([EmployeeID])
GO
ALTER TABLE [dbo].[EmployeeRecords] CHECK CONSTRAINT [FK_EmployeeRecords_Employees]
GO
ALTER TABLE [dbo].[EmployeeRecords]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeRecords_Entities] FOREIGN KEY([EventID])
REFERENCES [dbo].[Entities] ([EntityID])
GO
ALTER TABLE [dbo].[EmployeeRecords] CHECK CONSTRAINT [FK_EmployeeRecords_Entities]
GO
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_EmployeeRecords] FOREIGN KEY([EmployeeRecordID])
REFERENCES [dbo].[EmployeeRecords] ([EmployeeRecordID])
GO
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_EmployeeRecords]
GO
ALTER TABLE [dbo].[Entities]  WITH CHECK ADD  CONSTRAINT [FK_Entities_Employees] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employees] ([EmployeeID])
GO
ALTER TABLE [dbo].[Entities] CHECK CONSTRAINT [FK_Entities_Employees]
GO
ALTER TABLE [dbo].[Entities]  WITH CHECK ADD  CONSTRAINT [FK_Entities_EntityTypes] FOREIGN KEY([EntityTypeID])
REFERENCES [dbo].[EntityTypes] ([EntityTypeID])
GO
ALTER TABLE [dbo].[Entities] CHECK CONSTRAINT [FK_Entities_EntityTypes]
GO
ALTER TABLE [dbo].[EntityFields]  WITH CHECK ADD  CONSTRAINT [FK_EntityFields_Entities] FOREIGN KEY([EntityID])
REFERENCES [dbo].[Entities] ([EntityID])
GO
ALTER TABLE [dbo].[EntityFields] CHECK CONSTRAINT [FK_EntityFields_Entities]
GO
ALTER TABLE [dbo].[EntityFields]  WITH CHECK ADD  CONSTRAINT [FK_EntityFields_FieldTypes] FOREIGN KEY([FieldTypeID])
REFERENCES [dbo].[FieldTypes] ([FieldTypeID])
GO
ALTER TABLE [dbo].[EntityFields] CHECK CONSTRAINT [FK_EntityFields_FieldTypes]
GO
ALTER TABLE [dbo].[FieldTypes]  WITH CHECK ADD  CONSTRAINT [FK_FieldTypes_DataTypes] FOREIGN KEY([DataTypeID])
REFERENCES [dbo].[DataTypes] ([DataTypeID])
GO
ALTER TABLE [dbo].[FieldTypes] CHECK CONSTRAINT [FK_FieldTypes_DataTypes]
GO
INSERT [dbo].[DataTypes] ([DataTypeID], [Name]) VALUES (1, N'Text')
GO
INSERT [dbo].[DataTypes] ([DataTypeID], [Name]) VALUES (2, N'Date')
GO
INSERT [dbo].[DataTypes] ([DataTypeID], [Name]) VALUES (3, N'Decimal')
GO
INSERT [dbo].[DataTypes] ([DataTypeID], [Name]) VALUES (4, N'Boolean')
GO
INSERT [dbo].[DataTypes] ([DataTypeID], [Name]) VALUES (5, N'Person')
GO
INSERT [dbo].[FieldTypes] ([FieldTypeID], [Name], [DataTypeID]) VALUES (1, N'Job Title', 1)
GO
INSERT [dbo].[FieldTypes] ([FieldTypeID], [Name], [DataTypeID]) VALUES (2, N'Job Start Date', 2)
GO
INSERT [dbo].[FieldTypes] ([FieldTypeID], [Name], [DataTypeID]) VALUES (3, N'Hire Date', 2)
GO
INSERT [dbo].[EntityTypes] ([EntityTypeID], [Name]) VALUES (1, N'Job')
GO
INSERT [dbo].[EntityTypes] ([EntityTypeID], [Name]) VALUES (2, N'Address')
GO
INSERT [dbo].[EntityTypes] ([EntityTypeID], [Name]) VALUES (3, N'Employee General')
GO

/****** Object:  StoredProcedure [dbo].[usp_DeleteEntireDatabase]    Script Date: 5/5/2015 7:54:02 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_DeleteEntireDatabase]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION deleteDatabase;

	DELETE FROM EntityFields;

	ALTER TABLE EmployeeRecords NOCHECK CONSTRAINT all;
	ALTER TABLE EmployeeEntities NOCHECK CONSTRAINT all;
	ALTER TABLE Employees NOCHECK CONSTRAINT all;
	ALTER TABLE Entities NOCHECK CONSTRAINT all;

	DELETE FROM Employees;
	DELETE FROM EmployeeRecords;
	DELETE FROM Entities;
	DELETE FROM EmployeeEntities;

	ALTER TABLE EmployeeRecords WITH CHECK CHECK CONSTRAINT all;
	ALTER TABLE EmployeeEntities WITH CHECK CHECK CONSTRAINT all;
	ALTER TABLE Employees WITH CHECK CHECK CONSTRAINT all;
	ALTER TABLE Entities WITH CHECK CHECK CONSTRAINT all;

	DBCC CHECKIDENT ('dbo.EntityFields',RESEED, 0);
	DBCC CHECKIDENT ('dbo.Employees',RESEED, 0);
	DBCC CHECKIDENT ('dbo.EmployeeRecords',RESEED, 0);
	DBCC CHECKIDENT ('dbo.Entities',RESEED, 0);
	DBCC CHECKIDENT ('dbo.EmployeeEntities',RESEED, 0);

	COMMIT TRANSACTION deleteDatabase;

END

GO


