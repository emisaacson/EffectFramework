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
    /// <summary>
    /// Implementation of IPersistenceService for SQL Server and Entity Framework 7.
    /// </summary>
    [Serializable]
    public class EntityFrameworkPersistenceService : IPersistenceService
    {
        [NonSerialized]
        protected Logger _Log;
        protected Logger Log
        {
            get
            {
                if (_Log == null)
                {
                    _Log = new Logger(GetType().Name);
                }
                return _Log;
            }
        }
        private string ConnectionString;
        public EntityFrameworkPersistenceService(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }
        public ObjectIdentity SaveSingleField(EntityBase Entity, FieldBase Field, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            try
            {
                if (ctx == null)
                {
                    db = new EntityFramework7DBContext(ConnectionString);
                }
                else
                {
                    db = (EntityFramework7DBContext)ctx;
                }

                if (Entity == null)
                {
                    throw new ArgumentNullException(nameof(Entity));
                }

                Field DbField = null;
                bool CreatedAnew = false;
                if (!Field.FieldID.HasValue)
                {
                    if (!Entity.EntityID.HasValue)
                    {
                        throw new ArgumentException("Must persist the entity to the database before the field.");
                    }

                    if (((IField)Field).Value == null ||
                        (Field.Type.DataType == DataType.Date && (DateTime)((IField)Field).Value == default(DateTime)) ||
                        (Field.Type.DataType == DataType.Lookup && (int)((IField)Field).Value == default(int)))
                    {
                        return null;
                    }

                    DbField = new Field()
                    {
                        IsDeleted = false,
                        FieldTypeID = Field.Type.Value,
                        EntityID = Entity.EntityID.Value,
                        Guid = Guid.NewGuid(),
                        CreateDate = DateTime.Now,
                        TenantID = Field.TenantID,
                    };
                    db.Fields.Add(DbField);
                    db.SaveChanges();
                    CreatedAnew = true;
                }
                else {
                    DbField = db.Fields.Where(ef => ef.FieldID == Field.FieldID.Value).FirstOrDefault();
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
                
                if (DbField.TenantID != Field.TenantID)
                {
                    Log.Fatal("Tenant ID Does not match. Database: {0}, Field Object: {1}", DbField.TenantID, Field.TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }

                if (!Field.Dirty)
                {
                    return new ObjectIdentity() {
                        ObjectID = Field.FieldID.Value,
                        ObjectGuid = Field.Guid,
                        DidUpdate = false,
                    };
                }

                if (((IField)Field).Value == null ||
                        (Field.Type.DataType == DataType.Date && (DateTime)((IField)Field).Value == default(DateTime)) ||
                        (Field.Type.DataType == DataType.Lookup && (int)((IField)Field).Value == default(int)))
                {
                    DbField.IsDeleted = true;
                    DbField.Guid = Guid.NewGuid();
                    DbField.DeleteDate = DateTime.Now;
                    db.SaveChanges();
                    return null;
                }

                DbField.ValueBoolean = null;
                DbField.ValueDate = null;
                DbField.ValueDecimal = null;
                DbField.ValueText = null;
                DbField.ValueLookup = null;
                DbField.ValueBinary = null;
                DbField.ValueEntityReference = null;
                DbField.ValueItemReference = null;

                DbField.Guid = Guid.NewGuid();

                if (Field.Type.DataType == DataType.Boolean)
                {
                    DbField.ValueBoolean = (bool?)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Date)
                {
                    DbField.ValueDate = (DateTime?)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Decimal)
                {
                    DbField.ValueDecimal = (decimal?)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Lookup)
                {
                    DbField.ValueLookup = (int?)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Text)
                {
                    DbField.ValueText = (string)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Binary)
                {
                    DbField.ValueBinary = (byte[])((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.EntityReference)
                {
                    DbField.ValueEntityReference = (int?)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.ItemReference)
                {
                    DbField.ValueItemReference = (int?)((IField)Field).Value;
                }

                db.SaveChanges();

                return new ObjectIdentity()
                {
                    ObjectID = DbField.FieldID,
                    ObjectGuid = DbField.Guid,
                    DidUpdate = true,
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
            EntityFramework7DBContext db = null;
            try {
                if (ctx == null)
                {
                    db = new EntityFramework7DBContext(ConnectionString);
                }
                else
                {
                    db = (EntityFramework7DBContext)ctx;
                }

                if (!Field.FieldID.HasValue)
                {
                    throw new InvalidOperationException("Must create a new field in the context of an entity.");
                }

                Field DbField = db.Fields.Where(ef => ef.FieldID == Field.FieldID.Value).FirstOrDefault();
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

                if (DbField.TenantID != Field.TenantID)
                {
                    Log.Fatal("Tenant ID Does not match. Database: {0}, Field Object: {1}", DbField.TenantID, Field.TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }

                if (!Field.Dirty)
                {
                    return new ObjectIdentity()
                    {
                        ObjectID = DbField.FieldID,
                        ObjectGuid = DbField.Guid,
                        DidUpdate = false,
                    };
                }

                if (((IField)Field).Value == null)
                {
                    DbField.IsDeleted = true;
                    DbField.Guid = Guid.NewGuid();
                    DbField.DeleteDate = DateTime.Now;
                    db.SaveChanges();
                    return null;
                }

                DbField.ValueBoolean = null;
                DbField.ValueDate = null;
                DbField.ValueDecimal = null;
                DbField.ValueText = null;
                DbField.ValueLookup = null;
                DbField.ValueEntityReference = null;

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
                else if (Field.Type.DataType == DataType.Lookup)
                {
                    DbField.ValueLookup = (int)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Text)
                {
                    DbField.ValueText = (string)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.EntityReference)
                {
                    DbField.ValueEntityReference = (int)((IField)Field).Value;
                }

                db.SaveChanges();

                return new ObjectIdentity()
                {
                    ObjectID = DbField.FieldID,
                    ObjectGuid = DbField.Guid,
                    DidUpdate = true,
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

        public ObjectIdentity SaveSingleEntity(Models.Item Item, EntityBase Entity, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            try
            {
                if (ctx == null)
                {
                    db = new EntityFramework7DBContext(ConnectionString);
                }
                else
                {
                    db = (EntityFramework7DBContext)ctx;
                }

                if (Item == null)
                {
                    throw new ArgumentNullException(nameof(Item));
                }

                Entity DbEntity = null;
                bool CreatedAnew = false;
                if (!Entity.EntityID.HasValue)
                {
                    if (!Item.ItemID.HasValue)
                    {
                        throw new ArgumentException("Must persist the item to the database before the entity.");
                    }
                    DbEntity = new Entity()
                    {
                        IsDeleted = false,
                        EntityTypeID = Entity.Type.Value,
                        EffectiveDate = Entity.EffectiveDate,
                        ItemID = Item.ItemID.Value,
                        Guid = Guid.NewGuid(),
                        CreateDate = DateTime.Now,
                        TenantID = Entity.TenantID,
                    };
                    db.Entities.Add(DbEntity);
                    db.SaveChanges();

                    CreatedAnew = true;
                }
                else
                {
                    DbEntity = db.Entities.Where(e => e.EntityID == Entity.EntityID.Value).FirstOrDefault();
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

                if (DbEntity.TenantID != Entity.TenantID)
                {
                    Log.Fatal("Tenant ID Does not match. Database: {0}, Entity Object: {1}", DbEntity.TenantID, Entity.TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }

                if (!Entity.Dirty)
                {
                    return new ObjectIdentity()
                    {
                        ObjectID = DbEntity.EntityID,
                        ObjectGuid = DbEntity.Guid,
                        DidUpdate = false,
                    };
                }


                DbEntity.EffectiveDate = Entity.EffectiveDate;
                DbEntity.EndEffectiveDate = Entity.EndEffectiveDate;
                DbEntity.IsDeleted = Entity.IsDeleted;
                DbEntity.Guid = Guid.NewGuid();

                if (Entity.IsDeleted)
                {
                    DbEntity.DeleteDate = DateTime.Now;
                }

                db.SaveChanges();

                return new ObjectIdentity()
                {
                    ObjectID = DbEntity.EntityID,
                    ObjectGuid = DbEntity.Guid,
                    DidUpdate = true,
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
            EntityFramework7DBContext db = null;
            try
            {
                if (ctx == null)
                {
                    db = new EntityFramework7DBContext(ConnectionString);
                }
                else
                {
                    db = (EntityFramework7DBContext)ctx;
                }

                Entity DbEntity = null;
                if (!Entity.EntityID.HasValue)
                {
                    throw new InvalidOperationException("Must create a new entity in the context of an item record.");
                }
                else
                {
                    DbEntity = db.Entities.Where(e => e.EntityID == Entity.EntityID.Value).FirstOrDefault();
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

                if (DbEntity.TenantID != Entity.TenantID)
                {
                    Log.Fatal("Tenant ID Does not match. Database: {0}, Entity Object: {1}", DbEntity.TenantID, Entity.TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }

                if (!Entity.Dirty)
                {
                    return new ObjectIdentity()
                    {
                        ObjectID = DbEntity.EntityID,
                        ObjectGuid = DbEntity.Guid,
                        DidUpdate = false,
                    };
                }


                DbEntity.EffectiveDate = Entity.EffectiveDate;
                DbEntity.EndEffectiveDate = Entity.EndEffectiveDate;
                DbEntity.IsDeleted = Entity.IsDeleted;
                DbEntity.Guid = Guid.NewGuid();

                if (Entity.IsDeleted)
                {
                    DbEntity.DeleteDate = DateTime.Now;
                }

                db.SaveChanges();

                return new ObjectIdentity()
                {
                    ObjectID = DbEntity.EntityID,
                    ObjectGuid = DbEntity.Guid,
                    DidUpdate = true,
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

        public ObjectIdentity SaveSingleItem(Models.Item Item, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            try
            {
                if (ctx == null)
                {
                    db = new EntityFramework7DBContext(ConnectionString);
                }
                else
                {
                    db = (EntityFramework7DBContext)ctx;
                }

                Models.Db.Item DbItem = null;
                bool CreatedAnew = false;
                if (!Item.ItemID.HasValue)
                {
                    DbItem = new Models.Db.Item()
                    {
                        IsDeleted = false,
                        ItemTypeID = Item.Type.Value,
                        Guid = Guid.NewGuid(),
                        TenantID = Item.TenantID,
                    };
                    db.Items.Add(DbItem);
                    db.SaveChanges();

                    CreatedAnew = true;
                }
                else
                {
                    DbItem = db.Items.Where(i => i.ItemID == Item.ItemID.Value).FirstOrDefault();
                }


                if (DbItem == null)
                {
                    throw new ArgumentException("The passed item ID is not valid.");
                }

                if (!CreatedAnew && DbItem.Guid != Item.Guid)
                {
                    throw new Exceptions.GuidMismatchException();
                }

                if (DbItem.ItemTypeID != Item.Type.Value)
                {
                    throw new Exceptions.DataTypeMismatchException();
                }

                if (DbItem.TenantID != Item.TenantID)
                {
                    Log.Fatal("Tenant ID Does not match. Database: {0}, Item Object: {1}", DbItem.TenantID, Item.TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }

                if (!Item.Dirty)
                {
                    return new ObjectIdentity()
                    {
                        ObjectID = DbItem.ItemID.Value,
                        ObjectGuid = DbItem.Guid,
                        DidUpdate = false,
                    };
                }

                DbItem.Guid = Guid.NewGuid();

                db.SaveChanges();

                return new ObjectIdentity()
                {
                    ObjectID = DbItem.ItemID.Value,
                    ObjectGuid = DbItem.Guid,
                    DidUpdate = true,
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

        public FieldBase RetreiveSingleFieldOrDefault(EntityBase Entity, Models.Fields.FieldType FieldType)
        {

            int FieldTypeID = FieldType.Value;

            return RetreiveSingleFieldOrDefault(Entity, FieldTypeID);
        }

        public FieldBase RetreiveSingleFieldOrDefault(EntityBase Entity, int FieldTypeID)
        {
            if (Entity == null)
            {
                throw new ArgumentNullException(nameof(Entity));
            }
            if (!Entity.EntityID.HasValue)
            {
                throw new ArgumentException("Cannot retrieve a field without an EntityID");
            }

            using (var db = new EntityFramework7DBContext(ConnectionString))
            {
                var DbField = db.Fields.Where(f => f.EntityID == Entity.EntityID.Value &&
                                              f.FieldTypeID == FieldTypeID &&
                                              !f.IsDeleted).FirstOrDefault();

                if (DbField == null)
                {
                    return null;
                }

                if (Entity.TenantID != DbField.TenantID)
                {
                    Log.Fatal("Tenant ID Does not match. EntityID: {0}, Entity Tenant ID: {1}, Field ID: {2} Field TenantID: {3}",
                        Entity.EntityID.Value, Entity.TenantID, DbField.FieldID, DbField.TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }


                FieldBase Base = new FieldBase(DbField);

                return Base;
            }
        }

        public FieldBase RetreiveSingleFieldOrDefault(int FieldID, IDbContext ctx = null)
        {

            EntityFramework7DBContext db = null;
            try
            {
                if (ctx == null)
                {
                    db = new EntityFramework7DBContext(ConnectionString);
                }
                else
                {
                    db = (EntityFramework7DBContext)ctx;
                }

                int TenantID = Configure.GetTenantResolutionProvider().GetTenantID();

                var DbField = db.Fields.Where(f => f.FieldID == FieldID &&
                                              f.TenantID == TenantID &&
                                              !f.IsDeleted).FirstOrDefault();

                if (DbField == null)
                {
                    return null;
                }

                FieldBase Base = new FieldBase(DbField);

                return Base;
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }
            }
        }

        public FieldBase RetreiveSingleFieldOrDefault<FieldT>(EntityBase Entity) where FieldT : IField, new()
        {

            FieldT Instance = new FieldT();
            int FieldTypeID = Instance.Type.DataType.Value;

            return RetreiveSingleFieldOrDefault(Entity, FieldTypeID);
        }

        /*
        public EntityT RetreiveSingleEntityOrDefault<EntityT>(Models.Item Item, DateTime? EffectiveDate = null) where EntityT : EntityBase, new()
        {
            if (Item == null)
            {
                throw new ArgumentNullException();
            }
            if (!Item.ItemID.HasValue)
            {
                throw new ArgumentException("Must pass an Item with a valid ID.");
            }

            EntityT Instance = new EntityT();
            Instance.PersistenceService = this;

            using (var db = new EntityFramework7DBContext(ConnectionString))
            using (db.Database.AsRelational().Connection.BeginTransaction())
            {

                var DbEntityPossibilities = db.Entities
                    .Where(e =>
                        e.ItemID == Item.ItemID.Value &&
                        e.EntityTypeID == Instance.Type.Value &&
                        !e.IsDeleted);

                if (EffectiveDate.HasValue)
                {
                    DbEntityPossibilities = DbEntityPossibilities.Where(e =>
                        e.EffectiveDate <= EffectiveDate.Value &&
                        (!e.EndEffectiveDate.HasValue || e.EndEffectiveDate.Value < EffectiveDate.Value)
                    );
                }

                var DbEntity = DbEntityPossibilities.FirstOrDefault();

                if (DbEntity != null)
                {
                    EntityT Output = EntityFactory.GenerateEntityFromDbObject<EntityT>(DbEntity);
                }

                return default(EntityT);
            }
        }*/

        public EntityBase RetreiveSingleEntityOrDefault(int EntityID, IDbContext ctx = null)
        {
            if (EntityID < 1)
            {
                throw new ArgumentException("Must pass an Item with a valid ID.");
            }
            EntityFramework7DBContext db = null;
            try
            {
                if (ctx == null)
                {
                    db = new EntityFramework7DBContext(ConnectionString);
                }
                else
                {
                    db = (EntityFramework7DBContext)ctx;
                }

                int TenantID = Configure.GetTenantResolutionProvider().GetTenantID();

                var DbEntityPossibility = db.Entities
                    .Where(e =>
                        e.EntityID == EntityID &&
                        e.TenantID == TenantID &&
                        !e.IsDeleted).FirstOrDefault();

                if (DbEntityPossibility == null)
                {
                    return null;
                }

                return EntityBase.GenerateEntityFromDbObject(DbEntityPossibility);
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }
            }
        }

        public void SaveAndDeleteSingleEntity(EntityBase Entity, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            try
            {
                if (ctx == null)
                {
                    db = new EntityFramework7DBContext(ConnectionString);
                }
                else
                {
                    db = (EntityFramework7DBContext)ctx;
                }

                if (Entity == null)
                {
                    throw new ArgumentNullException(nameof(Entity));
                }

                Entity DbEntity = null;
                if (!Entity.EntityID.HasValue)
                {
                    throw new InvalidOperationException("Cannot delete an entity without an ID.");
                }
                else
                {
                    DbEntity = db.Entities.Where(e => e.EntityID == Entity.EntityID.Value).FirstOrDefault();
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

                if (DbEntity.TenantID != Entity.TenantID)
                {
                    Log.Fatal("Tenant ID Does not match. EntityID: {0}, Entity Tenant ID: {1}, Db Entity ID: {2} Db Entity TenantID: {3}",
                        Entity.EntityID.Value, Entity.TenantID, DbEntity.EntityID, DbEntity.TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }

                DbEntity.EffectiveDate = Entity.EffectiveDate;
                DbEntity.EndEffectiveDate = Entity.EndEffectiveDate;
                DbEntity.Guid = Guid.NewGuid();
                DbEntity.IsDeleted = true;
                DbEntity.DeleteDate = DateTime.Now;

                db.SaveChanges();
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }
            }
        }

        public List<EntityBase> RetreiveAllEntities(Models.Item Item, DateTime? EffectiveDate = null)
        {
            if (Item == null)
            {
                throw new ArgumentNullException(nameof(Item));
            }

            if (!Item.ItemID.HasValue)
            {
                throw new ArgumentException("Record must have an ID.");
            }

            int TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
            if (Item.TenantID != TenantID)
            {
                    Log.Fatal("Tenant ID Does not match. ItemID: {0}, Item Tenant ID: {1}, Global Tenant ID: {2}",
                        Item.ItemID.Value, Item.TenantID, TenantID);
                    throw new Exceptions.FatalException("Data error.");
            }

            using (var db = new EntityFramework7DBContext(ConnectionString))
            using (db.Database.AsRelational().Connection.BeginTransaction())
            {

                var DbEntityPossibilities = db.Entities
                    .Where(e =>
                        e.ItemID == Item.ItemID.Value &&
                        !e.IsDeleted);


                if (EffectiveDate.HasValue)
                {
                    DbEntityPossibilities = DbEntityPossibilities.Where(e =>
                        e.EffectiveDate <= EffectiveDate.Value &&
                        (!e.EndEffectiveDate.HasValue || e.EndEffectiveDate.Value > EffectiveDate.Value)
                    );
                }

                var DbEntities = DbEntityPossibilities.ToList();

                List<EntityBase> Output = new List<EntityBase>();
                foreach (var DbEntity in DbEntities)
                {
                    if (DbEntity.TenantID != TenantID)
                    {
                        Log.Fatal("Tenant ID Does not match. Entity Tenant ID: {0}, Global Tenant ID: {1}",
                            DbEntity.TenantID, TenantID);
                        throw new Exceptions.FatalException("Data error.");
                    }
                    Output.Add(EntityBase.GenerateEntityFromDbObject(DbEntity, Item));
                }

                return Output;
            }
        }

        public Guid RetreiveGuidForItem(Models.Item Item)
        {
            using (var db = new EntityFramework7DBContext(ConnectionString))
            {
                int TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
                if (Item.TenantID != TenantID)
                {
                    Log.Fatal("Tenant ID Does not match. Item Tenant ID: {0}, Global Tenant ID: {1}",
                        Item.TenantID, TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }
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

        public IDbContext GetDbContext()
        {
            return new EntityFramework7DBContext(ConnectionString);
        }

        public IEnumerable<LookupEntry> GetChoicesForLookupField(FieldLookup Field, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            try
            {
                if (ctx == null)
                {
                    db = new EntityFramework7DBContext(ConnectionString);
                }
                else
                {
                    db = (EntityFramework7DBContext)ctx;
                }

                if (!Field.Type.LookupTypeID.HasValue)
                {
                    return new LookupEntry[0];
                }

                int TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
                if (TenantID != Field.TenantID)
                {
                    Log.Fatal("Tenant ID Does not match. Field Tenant ID: {0}, Global Tenant ID: {1}",
                        Field.TenantID, TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }

                var Lookups = db.Lookups.Where(l => l.LookupTypeID == (int)Field.Type.LookupTypeID.Value);

                List<LookupEntry> Output = new List<LookupEntry>();
                foreach (var Lookup in Lookups)
                {
                    if (TenantID != Lookup.TenantID)
                    {
                        Log.Fatal("Tenant ID Does not match. Field Tenant ID: {0}, Global Tenant ID: {1}",
                            Field.TenantID, TenantID);
                        throw new Exceptions.FatalException("Data error.");
                    }
                    Output.Add(new LookupEntry(Lookup.LookupID, Lookup.Value));
                }

                return Output;
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }
            }
        }

        public IEnumerable<CompleteItem> RetreiveCompleteItems(IEnumerable<int> ItemIDs, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            try
            {
                if (ctx == null)
                {
                    db = new EntityFramework7DBContext(ConnectionString);
                }
                else
                {
                    db = (EntityFramework7DBContext)ctx;
                }
                int TenantID = Configure.GetTenantResolutionProvider().GetTenantID();

                CompleteItem[] Items = db.CompleteItems.Where(i => ItemIDs.Contains(i.ItemID)).ToArray();

                foreach (var Item in Items)
                {
                    if (Item.ItemTenantID != TenantID ||
                        Item.ItemTypeTenantID != TenantID ||
                        Item.EntityTenantID != TenantID ||
                        Item.EntityTypeTenantID != TenantID ||
                        Item.FieldTypeTenantID != TenantID ||
                        Item.FieldTenantID != TenantID)
                    {
                        Log.Fatal("Tenant ID Does not match. Global Tenant ID: {0}, ItemTenantID: {1}, ItemTypeTenantID: {2}, EntityTenantID: {3}, EntityTypeTenantID: {4}, FieldTypeTenantID: {5}, FieltTenantID: {6}",
                            TenantID, Item.ItemTenantID, Item.ItemTypeTenantID, Item.EntityTenantID, Item.EntityTypeTenantID, Item.FieldTypeTenantID, Item.FieldTypeTenantID, Item.FieldTenantID);
                        throw new Exceptions.FatalException("Data error.");
                    }
                }

                return Items;
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }
            }
        }

        public void RecordAudit(FieldBase Field, int? ItemID, string Comment, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            try
            {
                if (ctx == null)
                {
                    db = new EntityFramework7DBContext(ConnectionString);
                }
                else
                {
                    db = (EntityFramework7DBContext)ctx;
                }

                if (Field == null)
                {
                    throw new ArgumentNullException(nameof(Field));
                }

                if (!Field.FieldID.HasValue || !Field.Dirty)
                {
                    // No need to record a new field, just changes
                    // to existing ones.
                    return;
                }

                if (!Field.Entity.EntityID.HasValue || !Field.Entity.ItemID.HasValue)
                {
                    Log.Error("Entity or Item not yet persisted for this field. FieldID: {0}", Field.FieldID.Value);
                    throw new InvalidOperationException("Entity or Item not yet persisted for this field.");
                }

                int TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
                if (TenantID != Field.TenantID)
                {
                    Log.Fatal("TenantID Does not match. Global TenantID {0}, Field TenantID: {0}", TenantID, Field.TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }

                AuditLog Audit = new AuditLog()
                {
                    ItemID = Field.Entity.ItemID.Value,
                    EntityID = Field.Entity.EntityID.Value,
                    FieldID = Field.FieldID.Value,
                    ValueTextOld = Field is FieldString ? ((FieldString)Field).OriginalValue : null,
                    ValueTextNew = Field is FieldString ? ((FieldString)Field).Value : null,
                    ValueDateOld = Field is FieldDate ? ((FieldDate)Field).OriginalValue : null,
                    ValueDateNew = Field is FieldDate ? ((FieldDate)Field).Value : null,
                    ValueBooleanOld = Field is FieldBool ? ((FieldBool)Field).OriginalValue : null,
                    ValueBooleanNew = Field is FieldBool ? ((FieldBool)Field).Value : null,
                    ValueBinaryOld = Field is FieldBinary ? ((FieldBinary)Field).OriginalValue : null,
                    ValueBinaryNew = Field is FieldBinary ? ((FieldBinary)Field).Value : null,
                    ValueLookupOld = Field is FieldLookup ? ((FieldLookup)Field).OriginalValue : null,
                    ValueLookupNew = Field is FieldLookup ? ((FieldLookup)Field).Value : null,
                    ValueEntityReferenceOld = Field is FieldEntityReference ? ((FieldEntityReference)Field).OriginalValue : null,
                    ValueEntityReferenceNew = Field is FieldEntityReference ? ((FieldEntityReference)Field).Value : null,
                    CreateDate = DateTime.Now,
                    ItemReference = ItemID,
                    Comment = Comment,
                    TenantID = Field.TenantID,
                };

                db.AuditLogs.Add(Audit);

                db.SaveChanges();

            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }
            }
        }

        public void RecordAudit(EntityBase Entity, int? ItemID, string Comment, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            try
            {
                if (ctx == null)
                {
                    db = new EntityFramework7DBContext(ConnectionString);
                }
                else
                {
                    db = (EntityFramework7DBContext)ctx;
                }

                if (Entity == null)
                {
                    throw new ArgumentNullException(nameof(Entity));
                }

                if (!Entity.EntityID.HasValue || !Entity.Dirty)
                {
                    // No need to record a new entity, just changes
                    // to existing ones.
                    return;
                }

                int TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
                if (TenantID != Entity.TenantID)
                {
                    Log.Fatal("TenantID Does not match. Global TenantID {0}, Entity TenantID: {0}", TenantID, Entity.TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }

                if (!Entity.ItemID.HasValue)
                {
                    Log.Error("Entity or Item not yet persisted for this entity. EntityID: {0}", Entity.EntityID.Value);
                    throw new InvalidOperationException("Item not yet persisted for this Entity.");
                }

                AuditLog Audit = new AuditLog()
                {
                    ItemID = Entity.ItemID.Value,
                    EntityID = Entity.EntityID.Value,
                    EffectiveDateOld = Entity.OriginalEffectiveDate,
                    EffectiveDateNew = Entity.EffectiveDate,
                    EndEffectiveDateOld = Entity.OriginalEndEffectiveDate,
                    EndEffectiveDateNew = Entity.EndEffectiveDate,
                    CreateDate = DateTime.Now,
                    ItemReference = ItemID,
                    Comment = Comment,
                    TenantID = Entity.TenantID,
                };

                db.AuditLogs.Add(Audit);

                db.SaveChanges();

            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }
            }
        }

        public IFieldTypeMeta GetFieldTypeMeta(int ItemTypeID, int EntityTypeID, int FieldTypeID, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            try
            {
                if (ctx == null)
                {
                    db = new EntityFramework7DBContext(ConnectionString);
                }
                else
                {
                    db = (EntityFramework7DBContext)ctx;
                }

                Models.Fields.FieldType FieldType = (Models.Fields.FieldType)FieldTypeID;
                Type SystemMetaType = FieldType.DataType.MetaType;

                int TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
                var DbFieldTypeMeta = db.FieldTypeMetas.Where(f => f.ItemTypeID == ItemTypeID && f.EntityTypeID == EntityTypeID && f.FieldTypeID == FieldTypeID).FirstOrDefault();

                if (DbFieldTypeMeta != null && TenantID != DbFieldTypeMeta.TenantID)
                {
                    Log.Fatal("TenantID Does not match. Global TenantID {0}, Field Type Meta TenantID: {1}", TenantID, DbFieldTypeMeta.TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }

                return (IFieldTypeMeta)SystemMetaType.GetConstructor(new Type[] { typeof(FieldTypeMeta) }).Invoke(new object[] { DbFieldTypeMeta });
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }
            }
        }
    }
}
