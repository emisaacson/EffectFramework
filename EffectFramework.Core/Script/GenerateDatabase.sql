/****** Object:  StoredProcedure [dbo].[usp_DeleteEntireDatabase]    Script Date: 6/6/2015 11:42:00 PM ******/
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

	DELETE FROM Fields;
	DELETE FROM Entities;
	DELETE FROM Items;

	DBCC CHECKIDENT ('dbo.Fields',RESEED, 0);
	DBCC CHECKIDENT ('dbo.Entities',RESEED, 0);
	DBCC CHECKIDENT ('dbo.Items',RESEED, 0);

	COMMIT TRANSACTION deleteDatabase;

END






GO
/****** Object:  Table [dbo].[DataTypes]    Script Date: 6/6/2015 11:42:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DataTypes](
	[DataTypeID] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
 CONSTRAINT [PK_DataTypes] PRIMARY KEY CLUSTERED 
(
	[DataTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Entities]    Script Date: 6/6/2015 11:42:00 PM ******/
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
/****** Object:  Table [dbo].[EntityTypes]    Script Date: 6/6/2015 11:42:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntityTypes](
	[EntityTypeID] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
 CONSTRAINT [PK_EntityTypes] PRIMARY KEY CLUSTERED 
(
	[EntityTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Fields]    Script Date: 6/6/2015 11:42:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Fields](
	[FieldID] [int] IDENTITY(1,1) NOT NULL,
	[FieldTypeID] [int] NOT NULL,
	[EntityID] [int] NOT NULL,
	[ValueText] [nvarchar](max) NULL,
	[ValueDate] [datetime] NULL,
	[ValueDecimal] [decimal](18, 4) NULL,
	[ValueBoolean] [bit] NULL,
	[ValueLookup] [int] NULL,
	[ValueBinary] [varbinary](max) NULL,
	[ValueItemReference] [int] NULL,
	[ValueEntityReference] [int] NULL,
	[IsDeleted] [bit] NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Fields] PRIMARY KEY CLUSTERED 
(
	[FieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FieldTypeMeta]    Script Date: 6/6/2015 11:42:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FieldTypeMeta](
	[FieldTypeMetaID] [int] IDENTITY(1,1) NOT NULL,
	[ItemTypeID] [int] NOT NULL,
	[EntityTypeID] [int] NOT NULL,
	[FieldTypeID] [int] NOT NULL,
	[IsRequired] [bit] NULL,
	[IsRequiredQuery] [varchar](max) NULL,
	[DecimalMin] [decimal](18, 4) NULL,
	[DecimalMinQuery] [varchar](max) NULL,
	[DecimalMax] [decimal](18, 4) NULL,
	[DecimalMaxQuery] [varchar](max) NULL,
	[DatetimeMin] [datetime] NULL,
	[DatetimeMinQuery] [varchar](max) NULL,
	[DatetimeMax] [datetime] NULL,
	[DatetimeMaxQuery] [varchar](max) NULL,
	[TextRegex] [varchar](max) NULL,
	[TextRegexQuery] [varchar](max) NULL,
 CONSTRAINT [PK_FieldTypeMeta] PRIMARY KEY CLUSTERED 
(
	[FieldTypeMetaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_FieldTypeMeta] UNIQUE NONCLUSTERED 
(
	[ItemTypeID] ASC,
	[EntityTypeID] ASC,
	[FieldTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FieldTypes]    Script Date: 6/6/2015 11:42:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FieldTypes](
	[FieldTypeID] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[DataTypeID] [int] NOT NULL,
	[LookupTypeID] [int] NULL,
 CONSTRAINT [PK_FieldTypes] PRIMARY KEY CLUSTERED 
(
	[FieldTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Items]    Script Date: 6/6/2015 11:42:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Items](
	[ItemID] [int] IDENTITY(1,1) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[ItemTypeID] [int] NOT NULL,
 CONSTRAINT [PK_Items] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemTypes]    Script Date: 6/6/2015 11:42:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemTypes](
	[ItemTypeID] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
 CONSTRAINT [PK_ItemTypes] PRIMARY KEY CLUSTERED 
(
	[ItemTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Lookups]    Script Date: 6/6/2015 11:42:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Lookups](
	[LookupID] [int] IDENTITY(1,1) NOT NULL,
	[Value] [varchar](max) NOT NULL,
	[LookupTypeID] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Lookups] PRIMARY KEY CLUSTERED 
(
	[LookupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LookupTypes]    Script Date: 6/6/2015 11:42:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LookupTypes](
	[LookupTypeID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](1024) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_LookupTypes] PRIMARY KEY CLUSTERED 
(
	[LookupTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[CompleteItems]    Script Date: 6/6/2015 11:42:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[CompleteItems] AS
SELECT
i.ItemID,
i.[Guid] as ItemGuid,
it.ItemTypeID,
it.Name as ItemTypeName,
en.EntityID,
et.EntityTypeID,
et.Name as EntityTypeName,
en.EffectiveDate as EntityEffectiveDate,
en.EndEffectiveDate as EntityEndEffectiveDate,
en.[Guid] as EntityGuid,
ft.FieldTypeID,
ft.Name as FieldTypeName,
dt.DataTypeID,
dt.Name as DataTypeName,
f.FieldID,
f.ValueText,
f.ValueDate,
f.ValueDecimal,
f.ValueBoolean,
f.ValueLookup,
f.ValueItemReference,
f.ValueEntityReference,
f.ValueBinary,
l.Value as LookupText,
f.[Guid] as EntityFieldGuid
FROM Items i
JOIN ItemTypes it on it.ItemTypeID = i.ItemTypeID
JOIN Entities en on en.ItemID = i.ItemID and en.IsDeleted = 0
JOIN EntityTypes et on et.EntityTypeID = en.EntityTypeID
JOIN Fields f on f.EntityID = en.EntityID and f.IsDeleted = 0
JOIN FieldTypes ft on ft.FieldTypeID = f.FieldTypeID
JOIN DataTypes dt on dt.DataTypeID = ft.DataTypeID
LEFT JOIN Lookups l on f.ValueLookup = l.LookupId and l.IsDeleted = 0
WHERE i.IsDeleted = 0









GO
/****** Object:  View [dbo].[CurrentItems]    Script Date: 6/6/2015 11:42:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE VIEW [dbo].[CurrentItems] AS
SELECT
i.ItemID,
i.[Guid] as ItemGuid,
it.ItemTypeID,
it.Name as ItemTypeName,
en.EntityID,
et.EntityTypeID,
et.Name as EntityTypeName,
en.EffectiveDate as EntityEffectiveDate,
en.EndEffectiveDate as EntityEndEffectiveDate,
en.[Guid] as EntityGuid,
ft.FieldTypeID,
ft.Name as FieldTypeName,
dt.DataTypeID,
dt.Name as DataTypeName,
f.FieldID,
f.ValueText,
f.ValueDate,
f.ValueDecimal,
f.ValueBoolean,
f.ValueLookup,
f.ValueItemReference,
f.ValueEntityReference,
f.ValueBinary,
l.Value as LookupText,
f.[Guid] as EntityFieldGuid
FROM Items i
JOIN ItemTypes it on it.ItemTypeID = i.ItemTypeID
JOIN Entities en on en.ItemID = i.ItemID and en.IsDeleted = 0
JOIN EntityTypes et on et.EntityTypeID = en.EntityTypeID
JOIN Fields f on f.EntityID = en.EntityID and f.IsDeleted = 0
JOIN FieldTypes ft on ft.FieldTypeID = f.FieldTypeID
JOIN DataTypes dt on dt.DataTypeID = ft.DataTypeID
LEFT JOIN Lookups l on f.ValueLookup = l.LookupId and l.IsDeleted = 0
WHERE i.IsDeleted = 0 AND en.EffectiveDate <= GETDATE() AND (en.EndEffectiveDate IS NULL OR en.EndEffectiveDate > GETDATE())








GO
ALTER TABLE [dbo].[Lookups] ADD  CONSTRAINT [DF_Lookups_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
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
ALTER TABLE [dbo].[Fields]  WITH CHECK ADD  CONSTRAINT [FK_Fields_Entities] FOREIGN KEY([EntityID])
REFERENCES [dbo].[Entities] ([EntityID])
GO
ALTER TABLE [dbo].[Fields] CHECK CONSTRAINT [FK_Fields_Entities]
GO
ALTER TABLE [dbo].[Fields]  WITH CHECK ADD  CONSTRAINT [FK_Fields_EntityReferences] FOREIGN KEY([ValueEntityReference])
REFERENCES [dbo].[Entities] ([EntityID])
GO
ALTER TABLE [dbo].[Fields] CHECK CONSTRAINT [FK_Fields_EntityReferences]
GO
ALTER TABLE [dbo].[Fields]  WITH CHECK ADD  CONSTRAINT [FK_Fields_FieldTypes] FOREIGN KEY([FieldTypeID])
REFERENCES [dbo].[FieldTypes] ([FieldTypeID])
GO
ALTER TABLE [dbo].[Fields] CHECK CONSTRAINT [FK_Fields_FieldTypes]
GO
ALTER TABLE [dbo].[Fields]  WITH CHECK ADD  CONSTRAINT [FK_Fields_ItemReferences] FOREIGN KEY([ValueItemReference])
REFERENCES [dbo].[Items] ([ItemID])
GO
ALTER TABLE [dbo].[Fields] CHECK CONSTRAINT [FK_Fields_ItemReferences]
GO
ALTER TABLE [dbo].[Fields]  WITH CHECK ADD  CONSTRAINT [FK_Fields_Lookups] FOREIGN KEY([ValueLookup])
REFERENCES [dbo].[Lookups] ([LookupID])
GO
ALTER TABLE [dbo].[Fields] CHECK CONSTRAINT [FK_Fields_Lookups]
GO
ALTER TABLE [dbo].[FieldTypeMeta]  WITH CHECK ADD  CONSTRAINT [FK_FieldTypeMeta_EntityTypes] FOREIGN KEY([EntityTypeID])
REFERENCES [dbo].[EntityTypes] ([EntityTypeID])
GO
ALTER TABLE [dbo].[FieldTypeMeta] CHECK CONSTRAINT [FK_FieldTypeMeta_EntityTypes]
GO
ALTER TABLE [dbo].[FieldTypeMeta]  WITH CHECK ADD  CONSTRAINT [FK_FieldTypeMeta_FieldTypes] FOREIGN KEY([FieldTypeID])
REFERENCES [dbo].[FieldTypes] ([FieldTypeID])
GO
ALTER TABLE [dbo].[FieldTypeMeta] CHECK CONSTRAINT [FK_FieldTypeMeta_FieldTypes]
GO
ALTER TABLE [dbo].[FieldTypeMeta]  WITH CHECK ADD  CONSTRAINT [FK_FieldTypeMeta_ItemTypes] FOREIGN KEY([ItemTypeID])
REFERENCES [dbo].[ItemTypes] ([ItemTypeID])
GO
ALTER TABLE [dbo].[FieldTypeMeta] CHECK CONSTRAINT [FK_FieldTypeMeta_ItemTypes]
GO
ALTER TABLE [dbo].[FieldTypes]  WITH CHECK ADD  CONSTRAINT [FK_FieldTypes_DataTypes] FOREIGN KEY([DataTypeID])
REFERENCES [dbo].[DataTypes] ([DataTypeID])
GO
ALTER TABLE [dbo].[FieldTypes] CHECK CONSTRAINT [FK_FieldTypes_DataTypes]
GO
ALTER TABLE [dbo].[FieldTypes]  WITH CHECK ADD  CONSTRAINT [FK_FieldTypes_LookupTypes] FOREIGN KEY([LookupTypeID])
REFERENCES [dbo].[LookupTypes] ([LookupTypeID])
GO
ALTER TABLE [dbo].[FieldTypes] CHECK CONSTRAINT [FK_FieldTypes_LookupTypes]
GO
ALTER TABLE [dbo].[Items]  WITH CHECK ADD  CONSTRAINT [FK_Items_ItemTypes] FOREIGN KEY([ItemTypeID])
REFERENCES [dbo].[ItemTypes] ([ItemTypeID])
GO
ALTER TABLE [dbo].[Items] CHECK CONSTRAINT [FK_Items_ItemTypes]
GO
ALTER TABLE [dbo].[Lookups]  WITH CHECK ADD  CONSTRAINT [FK_Lookups_LookupTypes] FOREIGN KEY([LookupTypeID])
REFERENCES [dbo].[LookupTypes] ([LookupTypeID])
GO
ALTER TABLE [dbo].[Lookups] CHECK CONSTRAINT [FK_Lookups_LookupTypes]
GO





INSERT [dbo].[DataTypes] ([DataTypeID], [Name]) VALUES (1, N'Text')
GO
INSERT [dbo].[DataTypes] ([DataTypeID], [Name]) VALUES (2, N'Date')
GO
INSERT [dbo].[DataTypes] ([DataTypeID], [Name]) VALUES (3, N'Decimal')
GO
INSERT [dbo].[DataTypes] ([DataTypeID], [Name]) VALUES (4, N'Boolean')
GO
INSERT [dbo].[DataTypes] ([DataTypeID], [Name]) VALUES (5, N'Lookup')
GO
INSERT [dbo].[DataTypes] ([DataTypeID], [Name]) VALUES (6, N'Binary')
GO
INSERT [dbo].[DataTypes] ([DataTypeID], [Name]) VALUES (7, N'Item Reference')
GO
INSERT [dbo].[DataTypes] ([DataTypeID], [Name]) VALUES (8, N'Entity Reference')
GO