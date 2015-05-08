using System;
using System.Collections.Generic;
using EffectFramework.Core.Models;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Models.Fields;

namespace EffectFramework.Core.Services
{
    public interface IPersistenceService
    {
        Models.Db.ObjectIdentity SaveSingleField(EntityBase Entity, FieldBase Field, Models.Db.IDbContext ctx = null);
        Models.Db.ObjectIdentity SaveSingleField(FieldBase Field, Models.Db.IDbContext ctx = null);

        Models.Db.ObjectIdentity SaveSingleEntity(ItemRecord ItemRecord, EntityBase Entity, Models.Db.IDbContext ctx = null);
        Models.Db.ObjectIdentity SaveSingleEntity(EntityBase Entity, Models.Db.IDbContext ctx = null);

        FieldBase RetreiveSingleFieldOrDefault(EntityBase Entity, FieldType FieldType);
        FieldBase RetreiveSingleFieldOrDefault(EntityBase Entity, int FieldTypeID);
        FieldBase RetreiveSingleFieldOrDefault(int FieldID);

        FieldBase RetreiveSingleFieldOrDefault<FieldT>(EntityBase Entity) where FieldT : IField, new();

        EntityT RetreiveSingleEntityOrDefault<EntityT>(Models.ItemRecord ItemRecord) where EntityT : EntityBase, new();
        EntityT RetreiveSingleEntityOrDefault<EntityT>(int ItemRecordID) where EntityT : EntityBase, new();

        List<ItemRecord> RetreiveAllItemRecords(Item Item);
        List<ItemRecord> RetreiveAllItemRecords(int ItemID);

        Models.Db.ItemRecord RetreiveSingleDbItemRecord(int ItemRecordID);

        Guid RetreiveGuidForItemRecord(Models.Item Item);

        List<EntityBase> RetreiveAllEntities(Models.ItemRecord ItemRecord);
        List<EntityBase> RetreiveAllEntities(int ItemRecordID);
    }
}
