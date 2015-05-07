/****** Object:  Table [dbo].[DataTypes]    Script Date: 5/7/2015 5:47:43 PM ******/
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
/****** Object:  Table [dbo].[Entities]    Script Date: 5/7/2015 5:47:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Entities](
	[EntityID] [int] IDENTITY(1,1) NOT NULL,
	[EntityTypeID] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
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
/****** Object:  Table [dbo].[EntityFields]    Script Date: 5/7/2015 5:47:43 PM ******/
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
/****** Object:  Table [dbo].[EntityTypes]    Script Date: 5/7/2015 5:47:43 PM ******/
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
/****** Object:  Table [dbo].[FieldTypes]    Script Date: 5/7/2015 5:47:43 PM ******/
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
/****** Object:  Table [dbo].[ItemEntities]    Script Date: 5/7/2015 5:47:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemEntities](
	[ItemEntityID] [int] IDENTITY(1,1) NOT NULL,
	[ItemRecordID] [int] NOT NULL,
	[EntityID] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ItemEntities] PRIMARY KEY CLUSTERED 
(
	[ItemEntityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_ItemEntities] UNIQUE NONCLUSTERED 
(
	[ItemEntityID] ASC,
	[ItemRecordID] ASC,
	[EntityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemRecords]    Script Date: 5/7/2015 5:47:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemRecords](
	[ItemRecordID] [int] IDENTITY(1,1) NOT NULL,
	[ItemID] [int] NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[EndEffectiveDate] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ItemRecords] PRIMARY KEY CLUSTERED 
(
	[ItemRecordID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Items]    Script Date: 5/7/2015 5:47:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Items](
	[ItemID] [int] IDENTITY(1,1) NOT NULL,
	[ItemRecordID] [int] NULL,
	[IsDeleted] [bit] NOT NULL,
	[DisplayName] [varchar](1024) NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Items] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Entities]  WITH CHECK ADD  CONSTRAINT [FK_Entities_EntityTypes] FOREIGN KEY([EntityTypeID])
REFERENCES [dbo].[EntityTypes] ([EntityTypeID])
GO
ALTER TABLE [dbo].[Entities] CHECK CONSTRAINT [FK_Entities_EntityTypes]
GO
ALTER TABLE [dbo].[Entities]  WITH CHECK ADD  CONSTRAINT [FK_Entities_Items] FOREIGN KEY([ItemID])
REFERENCES [dbo].[Items] ([ItemID])
GO
ALTER TABLE [dbo].[Entities] CHECK CONSTRAINT [FK_Entities_Items]
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
ALTER TABLE [dbo].[ItemEntities]  WITH CHECK ADD  CONSTRAINT [FK_ItemEntities_Entities] FOREIGN KEY([EntityID])
REFERENCES [dbo].[Entities] ([EntityID])
GO
ALTER TABLE [dbo].[ItemEntities] CHECK CONSTRAINT [FK_ItemEntities_Entities]
GO
ALTER TABLE [dbo].[ItemEntities]  WITH CHECK ADD  CONSTRAINT [FK_ItemEntities_ItemRecords] FOREIGN KEY([ItemRecordID])
REFERENCES [dbo].[ItemRecords] ([ItemRecordID])
GO
ALTER TABLE [dbo].[ItemEntities] CHECK CONSTRAINT [FK_ItemEntities_ItemRecords]
GO
ALTER TABLE [dbo].[ItemRecords]  WITH CHECK ADD  CONSTRAINT [FK_ItemRecords_Items] FOREIGN KEY([ItemID])
REFERENCES [dbo].[Items] ([ItemID])
GO
ALTER TABLE [dbo].[ItemRecords] CHECK CONSTRAINT [FK_ItemRecords_Items]
GO
ALTER TABLE [dbo].[Items]  WITH CHECK ADD  CONSTRAINT [FK_Items_ItemRecords] FOREIGN KEY([ItemRecordID])
REFERENCES [dbo].[ItemRecords] ([ItemRecordID])
GO
ALTER TABLE [dbo].[Items] CHECK CONSTRAINT [FK_Items_ItemRecords]
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

CREATE VIEW [dbo].[CompleteItemRecord] AS
SELECT
i.ItemID,
i.[Guid] as ItemGuid,
it.ItemTypeID,
it.Name as ItemTypeName,
ir.ItemRecordID,
ir.EffectiveDate as ItemRecordEffectiveDate,
ir.EndEffectiveDate as ItemRecordEndEffectiveDate,
ir.[Guid] as ItemRecordGuid,
et.EntityTypeID,
et.Name as EntityTypeName,
ie.ItemEntityID,
ie.[Guid] as ItemEntityGuid,
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
FROM Items i
JOIN ItemTypes it on it.ItemTypeID = i.ItemTypeID
JOIN ItemRecords ir on ir.ItemID = i.ItemID and ir.IsDeleted = 0
JOIN ItemEntities ie on ie.ItemRecordID = ir.ItemRecordID and ie.IsDeleted = 0
JOIN Entities en on en.EntityID = ie.EntityID and en.IsDeleted = 0
JOIN EntityTypes et on et.EntityTypeID = en.EntityTypeID
JOIN EntityFields ef on ef.EntityID = en.EntityID and ef.IsDeleted = 0
JOIN FieldTypes ft on ft.FieldTypeID = ef.FieldTypeID
JOIN DataTypes dt on dt.DataTypeID = ft.DataTypeID


GO
/****** Object:  View [dbo].[CurrentItemRecord]    Script Date: 4/30/2015 2:45:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[CurrentItemRecord] AS
SELECT
i.ItemID,
i.[Guid] as ItemGuid,
it.ItemTypeID,
it.Name as ItemTypeName,
ir.ItemRecordID,
ir.EffectiveDate as ItemRecordEffectiveDate,
ir.EndEffectiveDate as ItemRecordEndEffectiveDate,
ir.[Guid] as ItemRecordGuid,
et.EntityTypeID,
et.Name as EntityTypeName,
ie.ItemEntityID,
ie.[Guid] as ItemEntityGuid,
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
FROM Items i
JOIN ItemTypes it on it.ItemTypeID = i.ItemTypeID
JOIN ItemRecords ir on ir.ItemID = i.ItemID and ir.ItemRecordID = i.ItemRecordID and ir.IsDeleted = 0
JOIN ItemEntities ie on ie.ItemRecordID = ir.ItemRecordID and ie.IsDeleted = 0
JOIN Entities en on en.EntityID = ie.EntityID and en.IsDeleted = 0
JOIN EntityTypes et on et.EntityTypeID = en.EntityTypeID
JOIN EntityFields ef on ef.EntityID = en.EntityID and ef.IsDeleted = 0
JOIN FieldTypes ft on ft.FieldTypeID = ef.FieldTypeID
JOIN DataTypes dt on dt.DataTypeID = ft.DataTypeID

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

	ALTER TABLE ItemRecords NOCHECK CONSTRAINT all;
	ALTER TABLE ItemEntities NOCHECK CONSTRAINT all;
	ALTER TABLE Items NOCHECK CONSTRAINT all;
	ALTER TABLE Entities NOCHECK CONSTRAINT all;

	DELETE FROM Items;
	DELETE FROM ItemRecords;
	DELETE FROM Entities;
	DELETE FROM ItemEntities;

	ALTER TABLE ItemRecords WITH CHECK CHECK CONSTRAINT all;
	ALTER TABLE ItemEntities WITH CHECK CHECK CONSTRAINT all;
	ALTER TABLE Items WITH CHECK CHECK CONSTRAINT all;
	ALTER TABLE Entities WITH CHECK CHECK CONSTRAINT all;

	DBCC CHECKIDENT ('dbo.EntityFields',RESEED, 0);
	DBCC CHECKIDENT ('dbo.Items',RESEED, 0);
	DBCC CHECKIDENT ('dbo.ItemRecords',RESEED, 0);
	DBCC CHECKIDENT ('dbo.Entities',RESEED, 0);
	DBCC CHECKIDENT ('dbo.ItemEntities',RESEED, 0);

	COMMIT TRANSACTION deleteDatabase;

END

GO


