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

        Models.Db.ObjectIdentity SaveSingleEntity(Item Item, EntityBase Entity, Models.Db.IDbContext ctx = null);
        Models.Db.ObjectIdentity SaveSingleEntity(EntityBase Entity, Models.Db.IDbContext ctx = null);

        Models.Db.ObjectIdentity SaveSingleItem(Item Item, Models.Db.IDbContext ctx = null);

        FieldBase RetreiveSingleFieldOrDefault(EntityBase Entity, FieldType FieldType);
        FieldBase RetreiveSingleFieldOrDefault(EntityBase Entity, int FieldTypeID);
        FieldBase RetreiveSingleFieldOrDefault(int FieldID);

        FieldBase RetreiveSingleFieldOrDefault<FieldT>(EntityBase Entity) where FieldT : IField, new();

        EntityT RetreiveSingleEntityOrDefault<EntityT>(Item Item, DateTime? EffectiveDate = null) where EntityT : EntityBase, new();

        Guid RetreiveGuidForItem(Item Item);

        List<EntityBase> RetreiveAllEntities(Item Item, DateTime? EffectiveDate = null);

        Models.Db.IDbContext GetDbContext();
    }
}
