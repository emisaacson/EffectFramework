using System;
using System.Collections.Generic;
using System.Linq;
using EffectFramework.Core.Models;
using EffectFramework.Core.Models.Db;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Models.Fields;
using Microsoft.Data.Entity;

namespace EffectFramework.Core.Services
{
    public class EntityFrameworkPersistenceService : IPersistenceService
    {
        public ObjectIdentity SaveSingleField(EntityBase Entity, FieldBase Field, IDbContext ctx = null)
        {
            ItemDb7Context db = null;
            try
            {
                if (ctx == null)
                {
                    db = new ItemDb7Context();
                }
                else
                {
                    db = (ItemDb7Context)ctx;
                }

                if (Entity == null)
                {
                    throw new ArgumentNullException();
                }

                EntityField DbField = null;
                bool CreatedAnew = false;
                if (!Field.FieldID.HasValue)
                {
                    if (!Entity.EntityID.HasValue)
                    {
                        throw new ArgumentException("Must persist the entity to the database before the field.");
                    }

                    DbField = new EntityField()
                    {
                        IsDeleted = false,
                        FieldTypeID = Field.Type.Value,
                        EntityID = Entity.EntityID.Value,
                        Guid = Guid.NewGuid(),
                    };
                    db.Fields.Add(DbField);
                    db.SaveChanges();
                    CreatedAnew = true;
                }
                else {
                    DbField = db.Fields.Where(ef => ef.EntityFieldID == Field.FieldID.Value && ef.IsDeleted == false).FirstOrDefault();
                }

                if (DbField == null)
                {
                    throw new ArgumentException("The passed field ID is not valid.");
                }

                if (!CreatedAnew && DbField.Guid != Field.Guid)
                {
                    throw new Exceptions.GuidMismatchException();
                }

                if (DbField.FieldTypeID != Field.Type.Value)
                {
                    throw new Exceptions.DataTypeMismatchException();
                }

                if (!Field.Dirty)
                {
                    return new ObjectIdentity() {
                        ObjectID = Field.FieldID.Value,
                        ObjectGuid = Field.Guid
                    };
                }

                if (((IField)Field).Value == null)
                {
                    DbField.IsDeleted = true;
                    DbField.Guid = Guid.NewGuid();
                    db.SaveChanges();
                    return new ObjectIdentity()
                    {
                        ObjectID = DbField.EntityFieldID,
                        ObjectGuid = DbField.Guid
                    };
                }

                DbField.ValueBoolean = null;
                DbField.ValueDate = null;
                DbField.ValueDecimal = null;
                DbField.ValueText = null;
                DbField.ValueUser = null;
                DbField.Guid = Guid.NewGuid();

                if (Field.Type.DataType == DataType.Boolean)
                {
                    DbField.ValueBoolean = (bool)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Date)
                {
                    DbField.ValueDate = (DateTime)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Decimal)
                {
                    DbField.ValueDecimal = (decimal)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Person)
                {
                    DbField.ValueUser = (int)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Text)
                {
                    DbField.ValueText = (string)((IField)Field).Value;
                }

                db.SaveChanges();

                return new ObjectIdentity()
                {
                    ObjectID = DbField.EntityFieldID,
                    ObjectGuid = DbField.Guid
                };
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }
            }
        }
        public ObjectIdentity SaveSingleField(FieldBase Field, IDbContext ctx = null)
        {
            ItemDb7Context db = null;
            try {
                if (ctx == null)
                {
                    db = new ItemDb7Context();
                }
                else
                {
                    db = (ItemDb7Context)ctx;
                }

                if (!Field.FieldID.HasValue)
                {
                    throw new InvalidOperationException("Must create a new field in the context of an entity.");
                }

                EntityField DbField = db.Fields.Where(ef => ef.EntityFieldID == Field.FieldID.Value && ef.IsDeleted == false).FirstOrDefault();
                if (DbField == null)
                {
                    throw new ArgumentException("The passed field ID is not valid.");
                }

                if (DbField.Guid != Field.Guid)
                {
                    throw new Exceptions.GuidMismatchException();
                }

                if (DbField.FieldTypeID != Field.Type.Value)
                {
                    throw new Exceptions.DataTypeMismatchException();
                }

                if (!Field.Dirty)
                {
                    return new ObjectIdentity()
                    {
                        ObjectID = DbField.EntityFieldID,
                        ObjectGuid = DbField.Guid
                    };
                }

                if (((IField)Field).Value == null)
                {
                    DbField.IsDeleted = true;
                    DbField.Guid = Guid.NewGuid();
                    db.SaveChanges();
                    return new ObjectIdentity()
                    {
                        ObjectID = DbField.EntityFieldID,
                        ObjectGuid = DbField.Guid
                    };
                }

                DbField.ValueBoolean = null;
                DbField.ValueDate = null;
                DbField.ValueDecimal = null;
                DbField.ValueText = null;
                DbField.ValueUser = null;
                DbField.Guid = Guid.NewGuid();

                if (Field.Type.DataType == DataType.Boolean)
                {
                    DbField.ValueBoolean = (bool)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Date)
                {
                    DbField.ValueDate = (DateTime)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Decimal)
                {
                    DbField.ValueDecimal = (decimal)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Person)
                {
                    DbField.ValueUser = (int)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Text)
                {
                    DbField.ValueText = (string)((IField)Field).Value;
                }

                db.SaveChanges();

                return new ObjectIdentity()
                {
                    ObjectID = DbField.EntityFieldID,
                    ObjectGuid = DbField.Guid
                };
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }
            }
        }

        public ObjectIdentity SaveSingleEntity(Models.ItemRecord ItemRecord, EntityBase Entity, IDbContext ctx = null)
        {
            ItemDb7Context db = null;
            try
            {
                if (ctx == null)
                {
                    db = new ItemDb7Context();
                }
                else
                {
                    db = (ItemDb7Context)ctx;
                }

                if (ItemRecord == null)
                {
                    throw new ArgumentNullException();
                }

                Entity DbEntity = null;
                bool CreatedAnew = false;
                if (!Entity.EntityID.HasValue)
                {
                    if (!ItemRecord.ItemRecordID.HasValue)
                    {
                        throw new ArgumentException("Must persist the item record to the database before the entity.");
                    }
                    DbEntity = new Entity()
                    {
                        IsDeleted = false,
                        EntityTypeID = Entity.Type.Value,
                        ItemID = ItemRecord.ItemID,
                        Guid = Guid.NewGuid(),
                    };
                    db.Entities.Add(DbEntity);
                    db.SaveChanges();

                    ItemEntity DbItemEntity = new ItemEntity()
                    {
                        ItemRecordID = ItemRecord.ItemRecordID.Value,
                        EntityID = DbEntity.EntityID,
                        Guid = Guid.NewGuid(),
                        IsDeleted = false,
                    };
                    db.ItemEntities.Add(DbItemEntity);
                    db.SaveChanges();
                    CreatedAnew = true;
                }
                else
                {
                    DbEntity = db.Entities.Where(e => e.EntityID == Entity.EntityID.Value && e.IsDeleted == false).FirstOrDefault();
                }


                if (DbEntity == null)
                {
                    throw new ArgumentException("The passed entity ID is not valid.");
                }

                if (!CreatedAnew && DbEntity.Guid != Entity.Guid)
                {
                    throw new Exceptions.GuidMismatchException();
                }

                if (DbEntity.EntityTypeID != Entity.Type.Value)
                {
                    throw new Exceptions.DataTypeMismatchException();
                }

                if (!Entity.Dirty)
                {
                    return new ObjectIdentity()
                    {
                        ObjectID = DbEntity.EntityID,
                        ObjectGuid = DbEntity.Guid
                    };
                }


                DbEntity.EffectiveDate = Entity.EffectiveDate;
                DbEntity.EndEffectiveDate = Entity.EndEffectiveDate;
                DbEntity.Guid = Guid.NewGuid();

                db.SaveChanges();

                return new ObjectIdentity()
                {
                    ObjectID = DbEntity.EntityID,
                    ObjectGuid = DbEntity.Guid
                };
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }
            }
        }

        public ObjectIdentity SaveSingleEntity(EntityBase Entity, IDbContext ctx = null)
        {
            ItemDb7Context db = null;
            try
            {
                if (ctx == null)
                {
                    db = new ItemDb7Context();
                }
                else
                {
                    db = (ItemDb7Context)ctx;
                }

                Entity DbEntity = null;
                if (!Entity.EntityID.HasValue)
                {
                    throw new InvalidOperationException("Must create a new entity in the context of an item record.");
                }
                else
                {
                    DbEntity = db.Entities.Where(e => e.EntityID == Entity.EntityID.Value && e.IsDeleted == false).FirstOrDefault();
                }


                if (DbEntity == null)
                {
                    throw new ArgumentException("The passed entity ID is not valid.");
                }

                if (DbEntity.Guid != Entity.Guid)
                {
                    throw new Exceptions.GuidMismatchException();
                }

                if (DbEntity.EntityTypeID != Entity.Type.Value)
                {
                    throw new Exceptions.DataTypeMismatchException();
                }

                if (!Entity.Dirty)
                {
                    return new ObjectIdentity()
                    {
                        ObjectID = DbEntity.EntityID,
                        ObjectGuid = DbEntity.Guid
                    };
                }


                DbEntity.EffectiveDate = Entity.EffectiveDate;
                DbEntity.EndEffectiveDate = Entity.EndEffectiveDate;
                DbEntity.Guid = Guid.NewGuid();

                db.SaveChanges();

                return new ObjectIdentity()
                {
                    ObjectID = DbEntity.EntityID,
                    ObjectGuid = DbEntity.Guid
                };
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }
            }
        }

        public FieldBase RetreiveSingleFieldOrDefault(EntityBase Entity, FieldType FieldType)
        {

            int FieldTypeID = FieldType.DataType.Value;

            return RetreiveSingleFieldOrDefault(Entity, FieldTypeID);
        }

        public FieldBase RetreiveSingleFieldOrDefault(EntityBase Entity, int FieldTypeID)
        {
            if (Entity == null)
            {
                throw new ArgumentNullException();
            }
            if (!Entity.EntityID.HasValue)
            {
                throw new ArgumentException("Cannot retrieve a field without an EntityID");
            }

            using (var db = new ItemDb7Context())
            {
                var DbField = db.Fields.Where(f => f.EntityID == Entity.EntityID.Value &&
                                              f.FieldTypeID == FieldTypeID &&
                                              !f.IsDeleted).FirstOrDefault();

                if (DbField == null)
                {
                    return null;
                }

                FieldBase Base = new FieldBase(DbField);

                return Base;
            }
        }

        public FieldBase RetreiveSingleFieldOrDefault(int FieldID)
        {

            using (var db = new ItemDb7Context())
            {
                var DbField = db.Fields.Where(f => f.EntityFieldID == FieldID &&
                                              !f.IsDeleted).FirstOrDefault();

                if (DbField == null)
                {
                    return null;
                }

                FieldBase Base = new FieldBase(DbField);

                return Base;
            }
        }

        public FieldBase RetreiveSingleFieldOrDefault<FieldT>(EntityBase Entity) where FieldT : IField, new()
        {

            FieldT Instance = new FieldT();
            int FieldTypeID = Instance.Type.DataType.Value;

            return RetreiveSingleFieldOrDefault(Entity, FieldTypeID);
        }

        public EntityT RetreiveSingleEntityOrDefault<EntityT>(Models.ItemRecord ItemRecord) where EntityT : EntityBase, new()
        {
            if (ItemRecord == null)
            {
                throw new ArgumentNullException();
            }
            if (!ItemRecord.ItemRecordID.HasValue)
            {
                throw new ArgumentException("Cannot fetch an entity from an item record with a null ID.");
            }

            return RetreiveSingleEntityOrDefault<EntityT>(ItemRecord.ItemRecordID.Value);
        }

        public EntityT RetreiveSingleEntityOrDefault<EntityT>(int ItemRecordID) where EntityT : EntityBase, new()
        {
            if (ItemRecordID < 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            EntityT Instance = new EntityT();
            Instance.PersistenceService = this;

            using (var db = new ItemDb7Context())
            using (db.Database.AsRelational().Connection.BeginTransaction())
            {
                var EntityIDs = db.ItemEntities
                    .Where(er =>
                        er.ItemRecordID == ItemRecordID &&
                        !er.IsDeleted)
                    .Select(er =>
                        er.EntityID
                    );

                var DbEntity = db.Entities
                    .Where(e =>
                        EntityIDs.Contains(e.EntityID) &&
                        e.EntityTypeID == Instance.Type.Value &&
                        !e.IsDeleted)
                    .FirstOrDefault();

                if (DbEntity != null)
                {
                    EntityT Output = EntityFactory.GenerateEntityFromDbObject<EntityT>(DbEntity);
                }

                return default(EntityT);
            }
        }

        public List<EntityBase> RetreiveAllEntities(Models.ItemRecord ItemRecord)
        {
            if (ItemRecord == null)
            {
                throw new ArgumentNullException();
            }
            if (!ItemRecord.ItemRecordID.HasValue)
            {
                throw new ArgumentException("Record must have an ID.");
            }

            return RetreiveAllEntities(ItemRecord.ItemRecordID.Value);
        }

        public List<EntityBase> RetreiveAllEntities(int ItemRecordID)
        {
            using (var db = new ItemDb7Context())
            {
                var EntityIDs = db.ItemEntities
                    .Where(er =>
                        er.ItemRecordID == ItemRecordID &&
                        !er.IsDeleted)
                    .Select(er =>
                        er.EntityID
                    ).ToArray();

                var DbEntities = db.Entities
                    .Where(e =>
                        EntityIDs.Contains(e.EntityID) &&
                        !e.IsDeleted)
                    .ToList();

                List<EntityBase> Output = new List<EntityBase>();
                foreach (var DbEntity in DbEntities)
                {
                    Output.Add(EntityFactory.GenerateEntityFromDbObject(DbEntity));
                }

                return Output;
            }
        }

        public List<Models.ItemRecord> RetreiveAllItemRecords(Models.Item Item)
        {
            if (Item == null)
            {
                throw new ArgumentNullException();
            }
            if (!Item.ItemID.HasValue)
            {
                throw new ArgumentException("Item must has a valid ID.");
            }

            return RetreiveAllItemRecords(Item.ItemID.Value);
        }

        public List<Models.ItemRecord> RetreiveAllItemRecords(int ItemID)
        {
            using (var db = new ItemDb7Context())
            {
                var DbItemRecords = db.ItemRecords
                    .Where(er =>
                        er.ItemID == ItemID &&
                        er.IsDeleted == false)
                    .ToList();

                List<Models.ItemRecord> Output = new List<Models.ItemRecord>();

                foreach (var DbItemRecord in DbItemRecords)
                {
                    var ItemRecord = new Models.ItemRecord(DbItemRecord, this);
                    Output.Add(ItemRecord);
                }

                return Output;
            }
        }

        public Models.Db.ItemRecord RetreiveSingleDbItemRecord(int ItemRecordID)
        {
            using (var db = new ItemDb7Context())
            {
                var DbItemRecord = db.ItemRecords
                    .Where(er =>
                        er.ItemRecordID == ItemRecordID &&
                        er.IsDeleted == false)
                    .FirstOrDefault();

                return DbItemRecord;
            }
        }

        public Guid RetreiveGuidForItemRecord(Models.Item Item)
        {
            using (var db = new ItemDb7Context())
            {
                var DbItemRecord = db.Items
                    .Where(e =>
                        e.ItemID == Item.ItemID &&
                        e.ItemTypeID == Item.Type.Value &&
                        e.IsDeleted == false)
                    .FirstOrDefault();

                if (DbItemRecord != null)
                {
                    return DbItemRecord.Guid;
                }

                throw new ArgumentException("Invalid item ID passed.");
            }
        }

    }
}
