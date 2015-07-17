using System;
using System.Collections.Generic;
using System.Linq;
using EffectFramework.Core.Models;
using EffectFramework.Core.Models.Db;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Models.Fields;

namespace EffectFramework.Core.Services
{
    /// <summary>
    /// Implementation of IPersistenceService for SQL Server and Entity Framework 7.
    /// </summary>
    public class EntityFrameworkPersistenceService : IPersistenceService
    {
        private Logger Log = new Logger(nameof(EntityFrameworkPersistenceService));

        private string ConnectionString;
        public EntityFrameworkPersistenceService(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }
        public ObjectIdentity SaveSingleField(EntityBase Entity, FieldBase Field, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            Field DbField = null;
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

                bool CreatedAnew = false;
                if (!Field.FieldID.HasValue)
                {
                    if (!Entity.EntityID.HasValue)
                    {
                        throw new ArgumentException("Must persist the entity to the database before the field.");
                    }

                    if (((IField)Field).Value == null ||
                        (Field.Type.DataType == DataType.Date && (DateTime)((IField)Field).Value == default(DateTime)) ||
                        (Field.Type.DataType == DataType.Lookup && (long)((IField)Field).Value == default(long)))
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
                    DbField = db.Fields.FirstOrDefault(ef => ef.FieldID == Field.FieldID.Value);
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
                        (Field.Type.DataType == DataType.Lookup && (long)((IField)Field).Value == default(long)))
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
                    DbField.ValueLookup = (long?)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Text)
                {
                    DbField.ValueText = (string)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Binary)
                {
                    DbField.ValueBinary = (byte[])((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Object)
                {
                    DbField.ValueBinary = (byte[])((IField)Field).DereferencedValue;
                }
                else if (Field.Type.DataType == DataType.EntityReference)
                {
                    DbField.ValueEntityReference = (long?)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.ItemReference)
                {
                    DbField.ValueItemReference = (long?)((IField)Field).Value;
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

                // EITODO: No idea why this is necessary
                if (db != null && DbField != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<Field>().Where(e => e.Entity == DbField).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }
        public ObjectIdentity SaveSingleField(FieldBase Field, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            Field DbField = null;
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

                DbField = db.Fields.FirstOrDefault(ef => ef.FieldID == Field.FieldID.Value);
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
                DbField.ValueItemReference = null;

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
                    DbField.ValueLookup = (long)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Text)
                {
                    DbField.ValueText = (string)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Binary)
                {
                    DbField.ValueBinary = (byte[])((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Object)
                {
                    DbField.ValueBinary = (byte[])((IField)Field).DereferencedValue;
                }
                else if (Field.Type.DataType == DataType.EntityReference)
                {
                    DbField.ValueEntityReference = (long)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.ItemReference)
                {
                    DbField.ValueItemReference = (long)((IField)Field).Value;
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

                // EITODO: No idea why this is necessary
                if (db != null && DbField != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<Field>().Where(e => e.Entity == DbField).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }

        public ObjectIdentity SaveSingleEntity(Models.Item Item, EntityBase Entity, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            Entity DbEntity = null;
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
                    DbEntity = db.Entities.FirstOrDefault(e => e.EntityID == Entity.EntityID.Value);
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

                // EITODO: No idea why this is necessary
                if (db != null && DbEntity != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<Entity>().Where(e => e.Entity == DbEntity).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }

        public ObjectIdentity SaveSingleEntity(EntityBase Entity, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            Entity DbEntity = null;
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

                if (!Entity.EntityID.HasValue)
                {
                    throw new InvalidOperationException("Must create a new entity in the context of an item record.");
                }
                else
                {
                    DbEntity = db.Entities.FirstOrDefault(e => e.EntityID == Entity.EntityID.Value);
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

                // EITODO: No idea why this is necessary
                if (db != null && DbEntity != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<Entity>().Where(e => e.Entity == DbEntity).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }

        public ObjectIdentity SaveSingleItem(Models.Item Item, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            Models.Db.Item DbItem = null;
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
                    DbItem = db.Items.FirstOrDefault(i => i.ItemID == Item.ItemID.Value);
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

                // EITODO: No idea why this is necessary
                if (db != null && DbItem != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<Models.Db.Item>().Where(e => e.Entity == DbItem).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }

        public FieldBase RetreiveSingleFieldOrDefault(EntityBase Entity, Models.Fields.FieldType FieldType, IDbContext ctx = null)
        {

            long FieldTypeID = FieldType.Value;

            return RetreiveSingleFieldOrDefault(Entity, FieldTypeID, ctx);
        }

        public FieldBase RetreiveSingleFieldOrDefault(EntityBase Entity, long FieldTypeID, IDbContext ctx = null)
        {
            if (Entity == null)
            {
                throw new ArgumentNullException(nameof(Entity));
            }
            if (!Entity.EntityID.HasValue)
            {
                throw new ArgumentException("Cannot retrieve a field without an EntityID");
            }

            EntityFramework7DBContext db = null;
            Field DbField = null;
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
                DbField = db.Fields.FirstOrDefault(f => f.EntityID == Entity.EntityID.Value &&
                                              f.FieldTypeID == FieldTypeID &&
                                              !f.IsDeleted);

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
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }
                
                // EITODO: No idea why this is necessary
                if (db != null && DbField != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<Field>().Where(e => e.Entity == DbField).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }

        public FieldBase RetreiveSingleFieldOrDefault(long FieldID, IDbContext ctx = null)
        {

            EntityFramework7DBContext db = null;
            Field DbField = null;
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

                long TenantID = Configure.GetTenantResolutionProvider().GetTenantID();

                DbField = db.Fields.FirstOrDefault(f => f.FieldID == FieldID &&
                                              f.TenantID == TenantID &&
                                              !f.IsDeleted);

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

                // EITODO: No idea why this is necessary
                if (db != null && DbField != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<Field>().Where(e => e.Entity == DbField).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }

        public FieldBase RetreiveSingleFieldOrDefault<FieldT>(EntityBase Entity, IDbContext ctx = null) where FieldT : IField, new()
        {

            FieldT Instance = new FieldT();
            long FieldTypeID = Instance.Type.DataType.Value;

            return RetreiveSingleFieldOrDefault(Entity, FieldTypeID, ctx);
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

        public EntityBase RetreiveSingleEntityOrDefault(long EntityID, IDbContext ctx = null)
        {
            if (EntityID < 1)
            {
                throw new ArgumentException("Must pass an Item with a valid ID.");
            }
            EntityFramework7DBContext db = null;
            Entity DbEntityPossibility = null;
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

                long TenantID = Configure.GetTenantResolutionProvider().GetTenantID();

                DbEntityPossibility = db.Entities
                    .FirstOrDefault(e =>
                        e.EntityID == EntityID &&
                        e.TenantID == TenantID &&
                        !e.IsDeleted);

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

                // EITODO: No idea why this is necessary
                if (db != null && DbEntityPossibility != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<Entity>().Where(e => e.Entity == DbEntityPossibility).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }

        public void SaveAndDeleteSingleEntity(EntityBase Entity, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            Entity DbEntity = null;
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

                if (!Entity.EntityID.HasValue)
                {
                    throw new InvalidOperationException("Cannot delete an entity without an ID.");
                }
                else
                {
                    DbEntity = db.Entities.FirstOrDefault(e => e.EntityID == Entity.EntityID.Value);
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

                // EITODO: No idea why this is necessary
                if (db != null && DbEntity != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<Entity>().Where(e => e.Entity == DbEntity).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }

        public List<EntityBase> RetreiveAllEntities(Models.Item Item, DateTime? EffectiveDate = null, IDbContext ctx = null)
        {
            if (Item == null)
            {
                throw new ArgumentNullException(nameof(Item));
            }

            if (!Item.ItemID.HasValue)
            {
                throw new ArgumentException("Record must have an ID.");
            }

            long TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
            if (Item.TenantID != TenantID)
            {
                    Log.Fatal("Tenant ID Does not match. ItemID: {0}, Item Tenant ID: {1}, Global Tenant ID: {2}",
                        Item.ItemID.Value, Item.TenantID, TenantID);
                    throw new Exceptions.FatalException("Data error.");
            }

            EntityFramework7DBContext db = null;
            List<Entity> DbEntities = null;
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
                using (db.BeginTransaction())
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

                    DbEntities = DbEntityPossibilities.ToList();

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
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }

                // EITODO: No idea why this is necessary
                if (db != null && DbEntities != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<Entity>().Where(e => DbEntities.Contains(e.Entity)).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }

        public Guid RetreiveGuidForItem(Models.Item Item, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            Models.Db.Item DbItemRecord = null;
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
                long TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
                if (Item.TenantID != TenantID)
                {
                    Log.Fatal("Tenant ID Does not match. Item Tenant ID: {0}, Global Tenant ID: {1}",
                        Item.TenantID, TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }
                DbItemRecord = db.Items
                    .FirstOrDefault(e =>
                        e.ItemID == Item.ItemID &&
                        e.ItemTypeID == Item.Type.Value &&
                        e.IsDeleted == false);

                if (DbItemRecord != null)
                {
                    return DbItemRecord.Guid;
                }

                throw new ArgumentException("Invalid item ID passed.");
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }

                // EITODO: No idea why this is necessary
                if (db != null && DbItemRecord != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<Models.Db.Item>().Where(e => e.Entity == DbItemRecord).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }

        public IDbContext GetDbContext()
        {
            return new EntityFramework7DBContext(ConnectionString);
        }

        public IEnumerable<LookupCollection> GetAllLookupCollections(IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            IEnumerable<LookupType> LookupTypes = null;
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

                long TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
                LookupTypes = db.LookupTypes.Where(lt => lt.IsDeleted == false && lt.TenantID == TenantID);
                List<LookupCollection> Output = new List<LookupCollection>();
                foreach (var LookupType in LookupTypes)
                {
                    Output.Add(new LookupCollection(LookupType.LookupTypeID));
                }

                return Output;
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }

                // EITODO: No idea why this is necessary
                if (db != null && LookupTypes != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<LookupType>().Where(e => LookupTypes.Contains(e.Entity)).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }
        public LookupCollection GetLookupCollectionById(long LookupTypeID, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            LookupType DbLookupType = null;
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

                long TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
                DbLookupType = db.LookupTypes.FirstOrDefault(lt => lt.LookupTypeID == LookupTypeID && lt.IsDeleted == false);

                if (DbLookupType == null)
                {
                    Log.Error("The passed LookupTypeID is not valid. LookupTypeID: {0}", LookupTypeID);
                    throw new ArgumentException("The passed LookupTypeID is not valid.");
                }

                if (DbLookupType.TenantID != TenantID)
                {
                    Log.Fatal("Tenant ID Does not match. Field Type Tenant ID: {0}, Global Tenant ID: {1}",
                        DbLookupType.TenantID, TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }

                return new LookupCollection(DbLookupType);
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }

                // EITODO: No idea why this is necessary
                if (db != null && DbLookupType != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<LookupType>().Where(e => e.Entity == DbLookupType).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }

        public ObjectIdentity SaveLookupCollection(LookupCollection LookupCollection, IDbContext ctx = null)
        {
            if (LookupCollection.IsReadOnly)
            {
                throw new InvalidOperationException("Cannot save a read only collection.");
            }
            EntityFramework7DBContext db = null;
            Models.Db.LookupType DbLookupType = null;
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

                bool CreatedAnew = false;
                if (!LookupCollection.LookupTypeID.HasValue)
                {
                    DbLookupType = new Models.Db.LookupType()
                    {
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                        Name = LookupCollection.Name,
                        TenantID = LookupCollection.TenantID,
                    };
                    db.LookupTypes.Add(DbLookupType);
                    db.SaveChanges();

                    CreatedAnew = true;
                }
                else
                {
                    DbLookupType = db.LookupTypes.FirstOrDefault(i => i.LookupTypeID == LookupCollection.LookupTypeID.Value && i.IsDeleted == false);
                }


                if (DbLookupType == null)
                {
                    throw new ArgumentException("The passed Lookup ID is not valid.");
                }

                if (!CreatedAnew && DbLookupType.Guid != LookupCollection.Guid)
                {
                    throw new Exceptions.GuidMismatchException();
                }

                if (DbLookupType.TenantID != LookupCollection.TenantID)
                {
                    Log.Fatal("Tenant ID Does not match. Database: {0}, Lookup Object: {1}", DbLookupType.TenantID, LookupCollection.TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }

                if (!LookupCollection.Dirty)
                {
                    return new ObjectIdentity()
                    {
                        ObjectID = DbLookupType.LookupTypeID,
                        ObjectGuid = DbLookupType.Guid,
                        DidUpdate = false,
                    };
                }

                DbLookupType.Guid = Guid.NewGuid();
                DbLookupType.Name = LookupCollection.Name;

                db.SaveChanges();

                return new ObjectIdentity()
                {
                    ObjectID = DbLookupType.LookupTypeID,
                    ObjectGuid = DbLookupType.Guid,
                    DidUpdate = true,
                };
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }

                // EITODO: No idea why this is necessary
                if (db != null && DbLookupType != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<LookupType>().Where(e => e.Entity == DbLookupType).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }

        public ObjectIdentity SaveSingleLookupEntry(LookupEntry LookupEntry, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            Models.Db.Lookup DbLookup = null;
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

                bool CreatedAnew = false;
                if (!LookupEntry.ID.HasValue)
                {
                    if (LookupEntry.LookupCollection == null || !LookupEntry.LookupCollection.LookupTypeID.HasValue)
                    {
                        Log.Error("No lookup collection set on lookup entry.");
                        throw new InvalidOperationException("Cannot create a lookup entry without a Lookup Collection with an ID.");
                    }

                    if (LookupEntry.LookupCollection.IsReadOnly)
                    {
                        throw new InvalidOperationException("Cannot save a read-only collection entry.");
                    }

                    DbLookup = new Models.Db.Lookup()
                    {
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                        TenantID = LookupEntry.TenantID,
                        Value = LookupEntry.Value,
                        LookupTypeID = LookupEntry.LookupCollection.LookupTypeID.Value
                    };
                    db.Lookups.Add(DbLookup);
                    db.SaveChanges();

                    CreatedAnew = true;
                }
                else
                {
                    if (LookupEntry.LookupCollection.IsReadOnly)
                    {
                        throw new InvalidOperationException("Cannot save read-only collection entry.");
                    }
                    DbLookup = db.Lookups.FirstOrDefault(i => i.LookupID == LookupEntry.ID.Value && i.IsDeleted == false);
                }

                if (DbLookup == null)
                {
                    throw new ArgumentException("The passed Lookup ID is not valid.");
                }

                if (!CreatedAnew && DbLookup.Guid != LookupEntry.Guid)
                {
                    throw new Exceptions.GuidMismatchException();
                }

                if (DbLookup.TenantID != LookupEntry.TenantID)
                {
                    Log.Fatal("Tenant ID Does not match. Database: {0}, Lookup Object: {1}", DbLookup.TenantID, LookupEntry.TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }

                if (!LookupEntry.Dirty)
                {
                    return new ObjectIdentity()
                    {
                        ObjectID = DbLookup.LookupID,
                        ObjectGuid = DbLookup.Guid,
                        DidUpdate = false,
                    };
                }

                DbLookup.Guid = Guid.NewGuid();
                DbLookup.Value = LookupEntry.Value;

                db.SaveChanges();

                return new ObjectIdentity()
                {
                    ObjectID = DbLookup.LookupID,
                    ObjectGuid = DbLookup.Guid,
                    DidUpdate = true,
                };
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }

                // EITODO: No idea why this is necessary
                if (db != null && DbLookup != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<Lookup>().Where(e => e.Entity == DbLookup).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }

        public void SaveAndDeleteLookupEntry(LookupEntry LookupEntry, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            Lookup DbLookup = null;
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

                if (!LookupEntry.ID.HasValue)
                {
                    throw new InvalidOperationException("Cannot delete Lookup with a null ID.");
                }
                DbLookup = db.Lookups.FirstOrDefault(i => i.LookupID == LookupEntry.ID.Value && i.IsDeleted == false);

                if (DbLookup == null)
                {
                    throw new ArgumentException("The passed Lookup ID is not valid.");
                }

                if (DbLookup.Guid != LookupEntry.Guid)
                {
                    throw new Exceptions.GuidMismatchException();
                }

                if (DbLookup.TenantID != LookupEntry.TenantID)
                {
                    Log.Fatal("Tenant ID Does not match. Database: {0}, Lookup Object: {1}", DbLookup.TenantID, LookupEntry.TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }


                DbLookup.Guid = Guid.NewGuid();
                DbLookup.Value = LookupEntry.Value;
                DbLookup.IsDeleted = true;

                db.SaveChanges();

            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }

                // EITODO: No idea why this is necessary
                if (db != null && DbLookup != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<Lookup>().Where(e => e.Entity == DbLookup).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }
        public void SaveAndDeleteLookupCollection(LookupCollection LookupCollection, IDbContext ctx = null)
        {
            if (LookupCollection.IsReadOnly)
            {
                throw new InvalidOperationException("Cannot save a read only collection.");
            }
            EntityFramework7DBContext db = null;
            LookupType DbLookupType = null;
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

                if (!LookupCollection.LookupTypeID.HasValue)
                {
                    throw new InvalidOperationException("Cannot delete Lookup Collection with a null ID.");
                }
                DbLookupType = db.LookupTypes.FirstOrDefault(i => i.LookupTypeID == LookupCollection.LookupTypeID.Value && i.IsDeleted == false);

                if (DbLookupType == null)
                {
                    throw new ArgumentException("The passed Lookup Type ID is not valid.");
                }

                if (DbLookupType.Guid != LookupCollection.Guid)
                {
                    throw new Exceptions.GuidMismatchException();
                }

                if (DbLookupType.TenantID != LookupCollection.TenantID)
                {
                    Log.Fatal("Tenant ID Does not match. Database: {0}, Lookup Object: {1}", DbLookupType.TenantID, LookupCollection.TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }


                DbLookupType.Guid = Guid.NewGuid();
                DbLookupType.Name = LookupCollection.Name;
                DbLookupType.IsDeleted = true;

                db.SaveChanges();

            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }

                // EITODO: No idea why this is necessary
                if (db != null && DbLookupType != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<LookupType>().Where(e => e.Entity == DbLookupType).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }

        public IEnumerable<LookupEntry> GetLookupEntries(long LookupTypeID, LookupCollection LookupCollection, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            IEnumerable<Lookup> DbLookups = null;
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

                long TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
                DbLookups = db.Lookups.Where(l => l.LookupTypeID == LookupTypeID && l.IsDeleted == false);

                List<LookupEntry> Output = new List<LookupEntry>();
                foreach (var DbLookup in DbLookups)
                {
                    if (DbLookup.TenantID != TenantID)
                    {
                        Log.Fatal("TenantID does not match. Lookup TenantID: {0}, Global TenantID: {0}",
                            DbLookup.TenantID, TenantID);
                        throw new Exceptions.FatalException("Data error.");
                    }
                    Output.Add(new LookupEntry(DbLookup.LookupID, DbLookup.Value, DbLookup.TenantID, DbLookup.Guid, LookupCollection));
                }

                return Output;
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }

                // EITODO: No idea why this is necessary
                if (db != null && DbLookups != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<Lookup>().Where(e => DbLookups.Contains(e.Entity)).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }

        public IEnumerable<CompleteItem> RetreiveCompleteItems(IEnumerable<long> ItemIDs, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            CompleteItem[] Items = null; 
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
                long TenantID = Configure.GetTenantResolutionProvider().GetTenantID();

                Items = db.CompleteItems.Where(i => ItemIDs.Contains(i.ItemID)).ToArray();

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

                // EITODO: No idea why this is necessary
                if (db != null && Items != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<CompleteItem>().Where(e => Items.Contains(e.Entity)).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }

        public void RecordAudit(FieldBase Field, long? ItemID, string Comment, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            AuditLog Audit = null;
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

                long TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
                if (TenantID != Field.TenantID)
                {
                    Log.Fatal("TenantID Does not match. Global TenantID {0}, Field TenantID: {0}", TenantID, Field.TenantID);
                    throw new Exceptions.FatalException("Data error.");
                }

                Audit = new AuditLog()
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
                    ValueBinaryOld = Field is FieldBinary ? ((FieldBinary)Field).OriginalValue : 
                                     Field.Type.DataType == DataType.Object ? (byte[])((IField)Field).OriginalDereferencedValue : null,
                    ValueBinaryNew = Field is FieldBinary ? ((FieldBinary)Field).Value :
                                     Field.Type.DataType == DataType.Object ? (byte[])((IField)Field).DereferencedValue : null,
                    ValueLookupOld = Field is FieldLookup ? ((FieldLookup)Field).OriginalValue : null,
                    ValueLookupNew = Field is FieldLookup ? ((FieldLookup)Field).Value : null,
                    ValueEntityReferenceOld = Field is FieldEntityReference ? ((FieldEntityReference)Field).OriginalValue : null,
                    ValueEntityReferenceNew = Field is FieldEntityReference ? ((FieldEntityReference)Field).Value : null,
                    ValueItemReferenceOld = Field is FieldItemReference ? ((FieldItemReference)Field).OriginalValue : null,
                    ValueItemReferenceNew = Field is FieldItemReference ? ((FieldItemReference)Field).Value : null,
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

                // EITODO: No idea why this is necessary
                if (db != null && Audit != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<AuditLog>().Where(e => e.Entity == Audit).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }

        public void RecordAudit(EntityBase Entity, long? ItemID, string Comment, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            AuditLog Audit = null;
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

                long TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
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

                Audit = new AuditLog()
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

                // EITODO: No idea why this is necessary
                if (db != null && Audit != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<AuditLog>().Where(e => e.Entity == Audit).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }

        public IFieldTypeMeta GetFieldTypeMeta(long ItemTypeID, long EntityTypeID, long FieldTypeID, IDbContext ctx = null)
        {
            EntityFramework7DBContext db = null;
            FieldTypeMeta DbFieldTypeMeta = null;
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

                long TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
                DbFieldTypeMeta = db.FieldTypeMetas.FirstOrDefault(f => f.ItemTypeID == ItemTypeID && f.EntityTypeID == EntityTypeID && f.FieldTypeID == FieldTypeID);

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

                // EITODO: No idea why this is necessary
                if (db != null && DbFieldTypeMeta != null && ctx != null)
                {
                    var Entries = db.ChangeTracker.Entries<FieldTypeMeta>().Where(e => e.Entity == DbFieldTypeMeta).ToArray();
                    for (var i = Entries.Length - 1; i >= 0; i--)
                    {
                        Entries[i].State = Microsoft.Data.Entity.EntityState.Detached;
                    }
                }
            }
        }
    }
}
