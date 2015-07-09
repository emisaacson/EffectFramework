/****** Object:  StoredProcedure [dbo].[usp_DeleteEntireDatabase]    Script Date: 6/29/2015 1:28:44 AM ******/
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
/****** Object:  Table [dbo].[AuditLog]    Script Date: 6/29/2015 1:28:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AuditLog](
	[AuditLogID] [int] IDENTITY(1,1) NOT NULL,
	[ItemID] [int] NOT NULL,
	[EntityID] [int] NOT NULL,
	[FieldID] [int] NULL,
	[EffectiveDateOld] [datetime] NULL,
	[EffectiveDateNew] [datetime] NULL,
	[EndEffectiveDateOld] [datetime] NULL,
	[EndEffectiveDateNew] [datetime] NULL,
	[ValueTextOld] [nvarchar](max) NULL,
	[ValueTextNew] [nvarchar](max) NULL,
	[ValueDateOld] [datetime] NULL,
	[ValueDateNew] [datetime] NULL,
	[ValueDecimalOld] [decimal](18, 4) NULL,
	[ValueDecimalNew] [decimal](18, 4) NULL,
	[ValueBooleanOld] [bit] NULL,
	[ValueBooleanNew] [bit] NULL,
	[ValueLookupOld] [int] NULL,
	[ValueLookupNew] [int] NULL,
	[ValueBinaryOld] [varbinary](max) NULL,
	[ValueBinaryNew] [varbinary](max) NULL,
	[ValueItemReferenceOld] [int] NULL,
	[ValueItemReferenceNew] [int] NULL,
	[ValueEntityReferenceOld] [int] NULL,
	[ValueEntityReferenceNew] [int] NULL,
	[CreateDate] [datetime] NOT NULL,
	[ItemReference] [int] NULL,
	[Comment] [nvarchar](max) NULL,
	[TenantID] [int] NOT NULL,
 CONSTRAINT [PK_AuditLog] PRIMARY KEY CLUSTERED 
(
	[AuditLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DataTypes]    Script Date: 6/29/2015 1:28:44 AM ******/
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
/****** Object:  Table [dbo].[Entities]    Script Date: 6/29/2015 1:28:44 AM ******/
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
	[CreateDate] [datetime] NULL,
	[CreateItemReference] [int] NULL,
	[CreateComment] [nvarchar](max) NULL,
	[DeleteDate] [datetime] NULL,
	[DeleteItemReference] [int] NULL,
	[DeleteItemComment] [nvarchar](max) NULL,
	[TenantID] [int] NOT NULL,
 CONSTRAINT [PK_Entities] PRIMARY KEY CLUSTERED 
(
	[EntityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EntityTypes]    Script Date: 6/29/2015 1:28:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntityTypes](
	[EntityTypeID] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[TenantID] [int] NOT NULL,
 CONSTRAINT [PK_EntityTypes] PRIMARY KEY CLUSTERED 
(
	[EntityTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Fields]    Script Date: 6/29/2015 1:28:44 AM ******/
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
	[CreateDate] [datetime] NOT NULL,
	[CreateItemReference] [int] NULL,
	[CreateComment] [nvarchar](max) NULL,
	[DeleteDate] [datetime] NULL,
	[DeleteItemReference] [int] NULL,
	[DeleteItemComment] [nvarchar](max) NULL,
	[TenantID] [int] NOT NULL,
 CONSTRAINT [PK_Fields] PRIMARY KEY CLUSTERED 
(
	[FieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FieldTypeMeta]    Script Date: 6/29/2015 1:28:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FieldTypeMeta](
	[FieldTypeMetaID] [int] IDENTITY(1,1) NOT NULL,
	[ItemTypeID] [int] NOT NULL,
	[EntityTypeID] [int] NOT NULL,
	[FieldTypeID] [int] NOT NULL,
	[IsRequired] [bit] NULL,
	[IsRequiredQuery] [nvarchar](max) NULL,
	[DecimalMin] [decimal](18, 4) NULL,
	[DecimalMinQuery] [nvarchar](max) NULL,
	[DecimalMax] [decimal](18, 4) NULL,
	[DecimalMaxQuery] [nvarchar](max) NULL,
	[DatetimeMin] [datetime] NULL,
	[DatetimeMinQuery] [nvarchar](max) NULL,
	[DatetimeMax] [datetime] NULL,
	[DatetimeMaxQuery] [nvarchar](max) NULL,
	[TextRegex] [nvarchar](max) NULL,
	[TextRegexQuery] [nvarchar](max) NULL,
	[TenantID] [int] NOT NULL,
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
/****** Object:  Table [dbo].[FieldTypes]    Script Date: 6/29/2015 1:28:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FieldTypes](
	[FieldTypeID] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[DataTypeID] [int] NOT NULL,
	[LookupTypeID] [int] NULL,
	[TenantID] [int] NOT NULL,
 CONSTRAINT [PK_FieldTypes] PRIMARY KEY CLUSTERED 
(
	[FieldTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Items]    Script Date: 6/29/2015 1:28:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Items](
	[ItemID] [int] IDENTITY(1,1) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[ItemTypeID] [int] NOT NULL,
	[TenantID] [int] NOT NULL,
 CONSTRAINT [PK_Items] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemTypes]    Script Date: 6/29/2015 1:28:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemTypes](
	[ItemTypeID] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[TenantID] [int] NOT NULL,
 CONSTRAINT [PK_ItemTypes] PRIMARY KEY CLUSTERED 
(
	[ItemTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Lookups]    Script Date: 6/29/2015 1:28:44 AM ******/
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
	[Guid] [uniqueidentifier] NOT NULL,
	[TenantID] [int] NOT NULL,
 CONSTRAINT [PK_Lookups] PRIMARY KEY CLUSTERED 
(
	[LookupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LookupTypes]    Script Date: 6/29/2015 1:28:44 AM ******/
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
	[Guid] [uniqueidentifier] NOT NULL,
	[TenantID] [int] NOT NULL,
	[IsReadOnly] [bit] NOT NULL CONSTRAINT [DF_LookupTypes_IsReadOnly]  DEFAULT ((0)),
 CONSTRAINT [PK_LookupTypes] PRIMARY KEY CLUSTERED 
(
	[LookupTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Tenants]    Script Date: 6/29/2015 1:28:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tenants](
	[TenantID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](1000) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Tenants] PRIMARY KEY CLUSTERED 
(
	[TenantID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  View [dbo].[CompleteItems]    Script Date: 6/29/2015 1:28:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CompleteItems] AS
SELECT
i.ItemID,
i.[Guid] as ItemGuid,
i.TenantID as ItemTenantID,
it.ItemTypeID,
it.Name as ItemTypeName,
it.TenantID as ItemTypeTenantID,
en.EntityID,
et.EntityTypeID,
et.Name as EntityTypeName,
et.TenantID as EntityTypeTenantID,
en.EffectiveDate as EntityEffectiveDate,
en.EndEffectiveDate as EntityEndEffectiveDate,
en.[Guid] as EntityGuid,
en.TenantID as EntityTenantID,
ft.FieldTypeID,
ft.Name as FieldTypeName,
ft.TenantID as FieldTypeTenantID,
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
f.[Guid] as EntityFieldGuid,
f.TenantID as FieldTenantID
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
/****** Object:  View [dbo].[CurrentItems]    Script Date: 6/29/2015 1:28:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CurrentItems] AS
SELECT * FROM [dbo].[CompleteItems]
WHERE EntityEffectiveDate <= GETDATE() AND (EntityEndEffectiveDate IS NULL OR EntityEndEffectiveDate > GETDATE())

GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [EntityFieldIndex]    Script Date: 6/29/2015 1:28:44 AM ******/
CREATE NONCLUSTERED INDEX [EntityFieldIndex] ON [dbo].[Fields]
(
	[EntityID] ASC,
	[IsDeleted] ASC
)
INCLUDE ( 	[FieldID],
	[FieldTypeID],
	[ValueText],
	[ValueDate],
	[ValueDecimal],
	[ValueBoolean],
	[ValueLookup],
	[ValueBinary],
	[ValueItemReference],
	[ValueEntityReference],
	[Guid],
	[TenantID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Lookups] ADD  CONSTRAINT [DF_Lookups_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[AuditLog]  WITH CHECK ADD  CONSTRAINT [FK_AuditLog_Entities] FOREIGN KEY([EntityID])
REFERENCES [dbo].[Entities] ([EntityID])
GO
ALTER TABLE [dbo].[AuditLog] CHECK CONSTRAINT [FK_AuditLog_Entities]
GO
ALTER TABLE [dbo].[AuditLog]  WITH CHECK ADD  CONSTRAINT [FK_AuditLog_Entities1] FOREIGN KEY([ValueEntityReferenceOld])
REFERENCES [dbo].[Entities] ([EntityID])
GO
ALTER TABLE [dbo].[AuditLog] CHECK CONSTRAINT [FK_AuditLog_Entities1]
GO
ALTER TABLE [dbo].[AuditLog]  WITH CHECK ADD  CONSTRAINT [FK_AuditLog_Entities2] FOREIGN KEY([ValueEntityReferenceNew])
REFERENCES [dbo].[Entities] ([EntityID])
GO
ALTER TABLE [dbo].[AuditLog] CHECK CONSTRAINT [FK_AuditLog_Entities2]
GO
ALTER TABLE [dbo].[AuditLog]  WITH CHECK ADD  CONSTRAINT [FK_AuditLog_Fields] FOREIGN KEY([FieldID])
REFERENCES [dbo].[Fields] ([FieldID])
GO
ALTER TABLE [dbo].[AuditLog] CHECK CONSTRAINT [FK_AuditLog_Fields]
GO
ALTER TABLE [dbo].[AuditLog]  WITH CHECK ADD  CONSTRAINT [FK_AuditLog_Items] FOREIGN KEY([ItemID])
REFERENCES [dbo].[Items] ([ItemID])
GO
ALTER TABLE [dbo].[AuditLog] CHECK CONSTRAINT [FK_AuditLog_Items]
GO
ALTER TABLE [dbo].[AuditLog]  WITH CHECK ADD  CONSTRAINT [FK_AuditLog_Items1] FOREIGN KEY([ValueItemReferenceOld])
REFERENCES [dbo].[Items] ([ItemID])
GO
ALTER TABLE [dbo].[AuditLog] CHECK CONSTRAINT [FK_AuditLog_Items1]
GO
ALTER TABLE [dbo].[AuditLog]  WITH CHECK ADD  CONSTRAINT [FK_AuditLog_Items2] FOREIGN KEY([ValueItemReferenceNew])
REFERENCES [dbo].[Items] ([ItemID])
GO
ALTER TABLE [dbo].[AuditLog] CHECK CONSTRAINT [FK_AuditLog_Items2]
GO
ALTER TABLE [dbo].[AuditLog]  WITH CHECK ADD  CONSTRAINT [FK_AuditLog_Items3] FOREIGN KEY([ItemReference])
REFERENCES [dbo].[Items] ([ItemID])
GO
ALTER TABLE [dbo].[AuditLog] CHECK CONSTRAINT [FK_AuditLog_Items3]
GO
ALTER TABLE [dbo].[AuditLog]  WITH CHECK ADD  CONSTRAINT [FK_AuditLog_Lookups] FOREIGN KEY([ValueLookupOld])
REFERENCES [dbo].[Lookups] ([LookupID])
GO
ALTER TABLE [dbo].[AuditLog] CHECK CONSTRAINT [FK_AuditLog_Lookups]
GO
ALTER TABLE [dbo].[AuditLog]  WITH CHECK ADD  CONSTRAINT [FK_AuditLog_Lookups1] FOREIGN KEY([ValueLookupNew])
REFERENCES [dbo].[Lookups] ([LookupID])
GO
ALTER TABLE [dbo].[AuditLog] CHECK CONSTRAINT [FK_AuditLog_Lookups1]
GO
ALTER TABLE [dbo].[AuditLog]  WITH CHECK ADD  CONSTRAINT [FK_AuditLog_Tenants] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenants] ([TenantID])
GO
ALTER TABLE [dbo].[AuditLog] CHECK CONSTRAINT [FK_AuditLog_Tenants]
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
ALTER TABLE [dbo].[Entities]  WITH CHECK ADD  CONSTRAINT [FK_Entities_Items1] FOREIGN KEY([CreateItemReference])
REFERENCES [dbo].[Items] ([ItemID])
GO
ALTER TABLE [dbo].[Entities] CHECK CONSTRAINT [FK_Entities_Items1]
GO
ALTER TABLE [dbo].[Entities]  WITH CHECK ADD  CONSTRAINT [FK_Entities_Items2] FOREIGN KEY([DeleteItemReference])
REFERENCES [dbo].[Items] ([ItemID])
GO
ALTER TABLE [dbo].[Entities] CHECK CONSTRAINT [FK_Entities_Items2]
GO
ALTER TABLE [dbo].[Entities]  WITH CHECK ADD  CONSTRAINT [FK_Entities_Tenants] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenants] ([TenantID])
GO
ALTER TABLE [dbo].[Entities] CHECK CONSTRAINT [FK_Entities_Tenants]
GO
ALTER TABLE [dbo].[EntityTypes]  WITH CHECK ADD  CONSTRAINT [FK_EntityTypes_Tenants] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenants] ([TenantID])
GO
ALTER TABLE [dbo].[EntityTypes] CHECK CONSTRAINT [FK_EntityTypes_Tenants]
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
ALTER TABLE [dbo].[Fields]  WITH CHECK ADD  CONSTRAINT [FK_Fields_Items] FOREIGN KEY([CreateItemReference])
REFERENCES [dbo].[Items] ([ItemID])
GO
ALTER TABLE [dbo].[Fields] CHECK CONSTRAINT [FK_Fields_Items]
GO
ALTER TABLE [dbo].[Fields]  WITH CHECK ADD  CONSTRAINT [FK_Fields_Items1] FOREIGN KEY([DeleteItemReference])
REFERENCES [dbo].[Items] ([ItemID])
GO
ALTER TABLE [dbo].[Fields] CHECK CONSTRAINT [FK_Fields_Items1]
GO
ALTER TABLE [dbo].[Fields]  WITH CHECK ADD  CONSTRAINT [FK_Fields_Lookups] FOREIGN KEY([ValueLookup])
REFERENCES [dbo].[Lookups] ([LookupID])
GO
ALTER TABLE [dbo].[Fields] CHECK CONSTRAINT [FK_Fields_Lookups]
GO
ALTER TABLE [dbo].[Fields]  WITH CHECK ADD  CONSTRAINT [FK_Fields_Tenants] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenants] ([TenantID])
GO
ALTER TABLE [dbo].[Fields] CHECK CONSTRAINT [FK_Fields_Tenants]
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
ALTER TABLE [dbo].[FieldTypeMeta]  WITH CHECK ADD  CONSTRAINT [FK_FieldTypeMeta_Tenants] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenants] ([TenantID])
GO
ALTER TABLE [dbo].[FieldTypeMeta] CHECK CONSTRAINT [FK_FieldTypeMeta_Tenants]
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
ALTER TABLE [dbo].[FieldTypes]  WITH CHECK ADD  CONSTRAINT [FK_FieldTypes_Tenants] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenants] ([TenantID])
GO
ALTER TABLE [dbo].[FieldTypes] CHECK CONSTRAINT [FK_FieldTypes_Tenants]
GO
ALTER TABLE [dbo].[Items]  WITH CHECK ADD  CONSTRAINT [FK_Items_ItemTypes] FOREIGN KEY([ItemTypeID])
REFERENCES [dbo].[ItemTypes] ([ItemTypeID])
GO
ALTER TABLE [dbo].[Items] CHECK CONSTRAINT [FK_Items_ItemTypes]
GO
ALTER TABLE [dbo].[Items]  WITH CHECK ADD  CONSTRAINT [FK_Items_Tenants] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenants] ([TenantID])
GO
ALTER TABLE [dbo].[Items] CHECK CONSTRAINT [FK_Items_Tenants]
GO
ALTER TABLE [dbo].[ItemTypes]  WITH CHECK ADD  CONSTRAINT [FK_ItemTypes_Tenants] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenants] ([TenantID])
GO
ALTER TABLE [dbo].[ItemTypes] CHECK CONSTRAINT [FK_ItemTypes_Tenants]
GO
ALTER TABLE [dbo].[Lookups]  WITH CHECK ADD  CONSTRAINT [FK_Lookups_LookupTypes] FOREIGN KEY([LookupTypeID])
REFERENCES [dbo].[LookupTypes] ([LookupTypeID])
GO
ALTER TABLE [dbo].[Lookups] CHECK CONSTRAINT [FK_Lookups_LookupTypes]
GO
ALTER TABLE [dbo].[Lookups]  WITH CHECK ADD  CONSTRAINT [FK_Lookups_Tenants] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenants] ([TenantID])
GO
ALTER TABLE [dbo].[Lookups] CHECK CONSTRAINT [FK_Lookups_Tenants]
GO
ALTER TABLE [dbo].[LookupTypes]  WITH CHECK ADD  CONSTRAINT [FK_LookupTypes_Tenants] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenants] ([TenantID])
GO
ALTER TABLE [dbo].[LookupTypes] CHECK CONSTRAINT [FK_LookupTypes_Tenants]
GO

----------------------------------------------
----------------------------------------------
----------------------------------------------



INSERT [dbo].[Tenants] ([Name], [IsDeleted]) VALUES (N'Default Tenant', 0)
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
INSERT [dbo].[DataTypes] ([DataTypeID], [Name]) VALUES (9, N'Object')
GO