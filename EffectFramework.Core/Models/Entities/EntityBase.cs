using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EffectFramework.Core.Models.Fields;
using EffectFramework.Core.Services;
using EffectFramework.Core.Models.Annotations;
using EffectFramework.Core.Exceptions;

namespace EffectFramework.Core.Models.Entities
{
    [Serializable]
    public abstract class EntityBase
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

        public abstract EntityType Type { get; }
        public int? EntityID { get; protected set; }
        public int? ItemID { get; protected set; }
        public Guid Guid { get; protected set; }
        public bool Dirty { get; protected set; }
        public virtual int TenantID { get; protected set; }

        private Item _Item;
        public Item Item {
            get {
                return _Item;
            }
            internal set
            {
                _Item = value;
                if (_Item != null)
                {
                    ItemID = _Item.ItemID;
                }
            }
        }
        internal bool FlagForRemoval { get; set; }
        public bool IsDeleted { get; protected set; }

        public DateTime OriginalEffectiveDate { get; protected set; }
        public DateTime? OriginalEndEffectiveDate { get; protected set; }

        private DateTime _EffectiveDate;
        public DateTime EffectiveDate {
            get {
                return this._EffectiveDate;
            }
            set
            {
                if (this._EffectiveDate != value)
                {
                    Log.Debug("Changing effective date: Old Value: {0}, New Value {1}",
                        ((object)this._EffectiveDate ?? (object)"null").ToString(),
                        ((object)value ?? "null").ToString());

                    this.Dirty = true;
                    this._EffectiveDate = value;
                }
            }
        }
        private DateTime? _EndEffectiveDate;
        public DateTime? EndEffectiveDate
        {
            get
            {
                return this._EndEffectiveDate;
            }
            set
            {
                if (this._EndEffectiveDate != value)
                {
                    Log.Debug("Changing end effective date: Old Value: {0}, New Value {1}",
                        ((object)this._EndEffectiveDate ?? (object)"null").ToString(),
                        ((object)value ?? "null").ToString());

                    this.Dirty = true;
                    this._EndEffectiveDate = value;
                }
            }
        }

        [NonSerialized]
        protected IPersistenceService _PersistenceService;
        protected IPersistenceService PersistenceService
        {
            get
            {
                if (_PersistenceService == null)
                {
                    _PersistenceService = Configure.GetPersistenceService();
                }
                return _PersistenceService;
            }
        }
        [NonSerialized]
        protected ICacheService _CacheService;
        protected ICacheService CacheService
        {
            get
            {
                if (_CacheService == null)
                {
                    _CacheService = Configure.GetCacheService();
                }
                return _CacheService;
            }
        }

        public UpdatePolicy GetUpdatePolicy()
        {
            var PolicyAttribute = this.GetType().GetCustomAttribute<ApplyPolicyAttribute>();

            if (PolicyAttribute == null)
            {
                return new NoOverlapPolicy();
            }

            return PolicyAttribute.Policy;
        }

        public IUpdateStrategy GetUpdateStrategy()
        {
            var DefaultStrategyAttribute = this.GetType().GetCustomAttribute<DefaultStrategyAttribute>();

            var UpdatePolicy = GetUpdatePolicy();
            IUpdateStrategy Strategy = null;

            if (DefaultStrategyAttribute == null)
            {
                return UpdatePolicy.GetDefaultStrategy();
            }

            Strategy = DefaultStrategyAttribute.Strategy;

            if (!UpdatePolicy.GetAvailableStrategies().Any(e => e.GetType() == Strategy.GetType()))
            {
                throw new InvalidOperationException("The specified strategy is not available for this Entity's policy.");
            }

            return Strategy;
        }

        public IUpdateStrategy GetUpdateStrategyForDuplicateDates()
        {
            var DefaultStrategyForDuplicateDatesAttribute = this.GetType().GetCustomAttribute<DefaultStrategyForDuplicateDatesAttribute>();

            var UpdatePolicy = GetUpdatePolicy();
            IUpdateStrategy Strategy = null;

            if (DefaultStrategyForDuplicateDatesAttribute == null)
            {
                return UpdatePolicy.GetDefaultStrategyForDuplicateDates();
            }

            Strategy = DefaultStrategyForDuplicateDatesAttribute.Strategy;

            if (!UpdatePolicy.GetAvailableStrategies().Any(e => e.GetType() == Strategy.GetType()))
            {
                throw new InvalidOperationException("The specified strategy is not available for this Entity's policy.");
            }

            return Strategy;
        }

        protected abstract void WireUpFields();

        public EntityBase(Item Item, Db.Entity DbEntity, Db.IDbContext ctx = null)
        {
            Log.Trace("Creating new EntityBase with PersistenceService and CacheService. Entity Type: {0}", this.Type.Name);

            this.Item = Item;
            this.Dirty = true;
            this.IsDeleted = false;
            this.FlagForRemoval = false;
            this.TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
            WireUpFields();

            if (DbEntity != null)
            {
                LoadUpEntity(DbEntity, ctx);
            }
        }

        protected void LoadUpEntity(Db.Entity DbEntity, Db.IDbContext ctx = null)
        {
            Log.Trace("Loading up entity fields from database. EntityID: {0}", DbEntity.EntityID);

            if (this.TenantID != DbEntity.TenantID)
            {
                Log.Fatal("TenantID Does not match. Entit Tenant ID: {0}, EntityId: {1}, DbEntity TenantID: {2}",
                    TenantID, DbEntity.EntityID, DbEntity.TenantID);
                throw new FatalException("Data error.");
            }

            this.EntityID = DbEntity.EntityID;
            this.ItemID = DbEntity.ItemID;
            this.Guid = DbEntity.Guid;
            this._EffectiveDate = DbEntity.EffectiveDate;
            this._EndEffectiveDate = DbEntity.EndEffectiveDate;
            this.IsDeleted = DbEntity.IsDeleted;

            var FieldObjects = GetAllEntityFields();

            foreach (var FieldObject in FieldObjects)
            {
                FieldObject.FillFromDatabase(this, FieldObject, ctx);
            }

            this.Dirty = false;

            RefreshOriginalValues();
        }

        public void LoadUpEntityFromView(IEnumerable<Db.CompleteItem> View)
        {
            if (View.Count() > 0)
            {
                var First = View.First();
                Log.Trace("Loading up entity fields from view. EntityID: {0}", First.EntityID);

                if (this.TenantID != First.EntityTenantID)
                {
                    Log.Fatal("TenantID Does not match. Entit Tenant ID: {0}, EntityId: {1}, DbEntity TenantID: {2}",
                        TenantID, First.EntityID, First.EntityTenantID);
                    throw new FatalException("Data error.");
                }

                this.EntityID         = First.EntityID;
                this.EffectiveDate    = First.EntityEffectiveDate;
                this.EndEffectiveDate = First.EntityEndEffectiveDate;
                this.ItemID           = First.ItemID;
                this.Guid             = First.EntityGuid;
                this.IsDeleted        = false;

                var Fields = GetAllEntityFields();
                foreach (var Row in View)
                {
                    var Field = Fields.Where(f => f.Type.Value == Row.FieldTypeID).FirstOrDefault();
                    if (Field != null)
                    {
                        Field.FillFromView(Row);
                    }
                }
            }

            this.Dirty = false;

            RefreshOriginalValues();
        }

        /// <summary>
        /// Copy all the values of all the fields from another entity.
        /// </summary>
        /// <param name="OtherEntity">The Entity to copy the field values from.</param>
        /// <exception cref="InvalidOperationException">Thrown if the passed entity is of a different type.</exception>
        public void CopyValuesFrom(EntityBase OtherEntity)
        {
            if (OtherEntity == null)
            {
                Log.Warn("Trying to copy values from a null entity. EntityID: {0}", this.EntityID.HasValue ? this.EntityID.Value.ToString() : "null");
                throw new ArgumentNullException(nameof(OtherEntity));
            }
            if (OtherEntity.Type != this.Type)
            {
                Log.Warn("Trying to copy values from a different entity type. EntityID: {0}, Other Entity ID: {1}, Entity Type: {3}, Other Entity Type: {4}",
                    this.EntityID.HasValue ? this.EntityID.Value.ToString() : "null",
                    OtherEntity.EntityID.HasValue ? OtherEntity.EntityID.Value.ToString() : "null",
                    this.Type.Name, OtherEntity.Type.Name);

                throw new InvalidOperationException("Must copy values from an entity of the same type.");
            }
            if (OtherEntity.TenantID != this.TenantID)
            {
                Log.Fatal("TenantID Does not match. This EntityID: {0}, This TenantID: {1}, Other Entity ID: {2}, Other TenantID: {3}",
                    this.EntityID, this.TenantID, OtherEntity.EntityID, OtherEntity.TenantID);
                throw new FatalException("Data error.");
            }

            Log.Trace("Copying values from another entity. EntityID: {0}, Other Entity ID: {1}",
                    this.EntityID.HasValue ? this.EntityID.Value.ToString() : "null",
                    OtherEntity.EntityID.HasValue ? OtherEntity.EntityID.Value.ToString() : "null");

            var Properties = OtherEntity.Type.Type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var Property in Properties)
            {
                if (typeof(IField).IsAssignableFrom(Property.PropertyType))
                {
                    IField OtherEntityField = (IField)Property.GetValue(OtherEntity);
                    IField ThisEntityField = (IField)Property.GetValue(this);
                    ThisEntityField.Value = OtherEntityField.Value;
                }
            }
        }

        private void Seppuku(Db.IDbContext ctx = null)
        {
            if (this.EntityID.HasValue && !this.FlagForRemoval)
            {
                PersistenceService.SaveAndDeleteSingleEntity(this, ctx);
            }
            this.FlagForRemoval = true;
        }

        /// <summary>
        /// Flags the Entity for deletion.
        /// </summary>
        public void Delete()
        {
            this.IsDeleted = true;
            this.Dirty = true;
        }

        /// <summary>
        /// Returns a list of all the field instances associated with this entity.
        /// </summary>
        /// <returns>The list of field instances of the entity.</returns>
        public List<FieldBase> GetAllEntityFields()
        {
            Type EntityType = this.GetType();

            PropertyInfo[] AllPublicProperties = EntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            List<FieldBase> Output = new List<FieldBase>();
            foreach (PropertyInfo Property in AllPublicProperties)
            {
                if (typeof(FieldBase).IsAssignableFrom(Property.PropertyType))
                {
                    Output.Add((FieldBase)Property.GetValue(this));
                }
            }

            return Output;
        }

        /// <summary>
        /// Returns a field object of the specified type if it exists, otherwise null.
        /// </summary>
        /// <param name="FieldType">The field type to search for</param>
        /// <returns>The field object, or null if not found.</returns>
        public FieldBase GetFieldByFieldType(FieldType FieldType)
        {
            return GetAllEntityFields().FirstOrDefault(f => f.Type == FieldType);
        }

        /// <summary>
        /// Returns true if two entities contain the save field values. It does not take
        /// Effective date fields or ID fields into the decision.
        /// </summary>
        /// <param name="OtherEntity">The other entity.</param>
        /// <returns>True if the entity values are identical, false otherwise.</returns>
        public bool IsIdenticalTo(EntityBase OtherEntity)
        {
            if (OtherEntity == null)
            {
                throw new ArgumentNullException(nameof(OtherEntity));
            }
            if (OtherEntity.Type != this.Type)
            {
                throw new InvalidOperationException("Cannot compare entities of different types.");
            }
            if (OtherEntity.TenantID != this.TenantID)
            {
                Log.Fatal("TenantID Does not match. This entity id: {0}, Other Entity ID: {1}, This TenantID: {2}, Other TenantID: {3}",
                    this.EntityID, OtherEntity.EntityID, this.TenantID, OtherEntity.TenantID);
                throw new FatalException("Data error.");
            }

            var OtherEntityFieldObjects = OtherEntity.GetAllEntityFields();
            bool AreIdentical = true;
            var FieldObjects = GetAllEntityFields();

            foreach (var OtherEntityField in OtherEntityFieldObjects)
            {
                var CurrentEntityField = FieldObjects.Where(f => f.Type == OtherEntityField.Type).Single();
                if (!CurrentEntityField.IsIdenticalTo(OtherEntityField))
                {
                    AreIdentical = false;
                    break;
                }
            }

            return AreIdentical;
        }

        /// <summary>
        /// Persist the entity to the database and all of its fields.
        /// </summary>
        /// <param name="ctx">An optional database context. One will be created if null.</param>
        /// <returns>true if the Entity or any of its fields were changed in the database.</returns>
        public bool PersistToDatabase(Db.IDbContext ctx = null)
        {
            if (!PerformSanityCheck())
            {
                Log.Fatal("TenantID mismatch. Global TenantID: {0}, Field TenantID: {1}, Item TenantID: {2}",
                    Configure.GetTenantResolutionProvider().GetTenantID(), this.TenantID, this.Item?.TenantID);
                throw new FatalException("Invalid data exception.");
            }

            PersistenceService.RecordAudit(this, null, null, ctx);

            Db.ObjectIdentity Identity;
            if (this.Item != null) {
                Identity = PersistenceService.SaveSingleEntity(Item, this, ctx);
            }
            else
            {
                Identity = PersistenceService.SaveSingleEntity(this, ctx);
            }
            this.Guid = Identity.ObjectGuid;
            this.EntityID = Identity.ObjectID;


            var FieldObjects = GetAllEntityFields();
            List<bool> Results = new List<bool>();
            foreach (var FieldObject in FieldObjects)
            {
                Results.Add(FieldObject.PersistToDatabase(ctx));
            }

            this.Dirty = false;

            bool ThisDidUpdate = Identity.DidUpdate || Results.Any(r => r == true);

            if (ThisDidUpdate)
            {
                CacheService.DeleteObject(string.Format("Entity:{0}", EntityID));
            }

            RefreshOriginalValues();

            if (this.IsDeleted)
            {
                this.FlagForRemoval = true;
            }
            else if (this.Item != null)
            {
                var PossiblePreviousEntities = Item.AllEntities
                    .Where(e => e.Type == this.Type &&
                                e.EndEffectiveDate.HasValue &&
                                e.EndEffectiveDate.Value == this.EffectiveDate &&
                                // This check is necessary to prevent two entities from each
                                // being possible previous entities of each other, which would
                                // cause infinite recursion.
                                (!this.EndEffectiveDate.HasValue || this.EndEffectiveDate.Value != e.EffectiveDate) &&
                                // Exclude the current entity
                                e != this);

                foreach (var PossiblePreviousEntity in PossiblePreviousEntities)
                {
                    if (PossiblePreviousEntity != null)
                    {
                        if (this.IsIdenticalTo(PossiblePreviousEntity))
                        {
                            PossiblePreviousEntity.EndEffectiveDate = this.EndEffectiveDate;
                            PossiblePreviousEntity.PersistToDatabase(ctx);
                            this.Seppuku(ctx);
                        }
                    }
                }
            }

            return ThisDidUpdate;
        }

        public bool PerformSanityCheck()
        {
            int _TenantID = Configure.GetTenantResolutionProvider().GetTenantID();

            if (_TenantID != this.TenantID)
            {
                return false;
            }
            if (this.Item != null && _TenantID != this.Item.TenantID)
            {
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Creates a new entity from the provided System Type. The system
        /// type must be a subclass of EntityBase.
        /// </summary>
        /// <param name="SystemType">The System Type</param>
        /// <param name="Item">An optional Item to bind to the Entity.</param>
        /// <param name="ctx">Database context (Optional)</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the passed SystemType is not a subclass of EntityBase</exception>
        /// <returns>The newly created Entity</returns>
        public static EntityBase GetEntityBySystemType(Type SystemType, Item Item = null, Db.IDbContext ctx = null)
        {
            if (SystemType == null)
            {
                throw new ArgumentNullException(nameof(SystemType));
            }
            if (!typeof(EntityBase).IsAssignableFrom(SystemType) || SystemType == typeof(EntityBase))
            {
                throw new ArgumentOutOfRangeException("Cannot create an entity from a type that isn't a subclass of EntityBase.");
            }

            return (EntityBase)Activator.CreateInstance(SystemType, new object[] { Item, (object)null, ctx });
        }

        /// <summary>
        /// Generates an entity from database object.
        /// </summary>
        /// <typeparam name="EntityT">The type of the Entity.</typeparam>
        /// <param name="DbEntity">The entity database object.</param>
        /// <param name="Item">An optional parent Item.</param>
        /// <param name="ctx">Database context (optional)</param>
        /// <returns>The generated Entity object.</returns>
        public static EntityT GenerateEntityFromDbObject<EntityT>(Db.Entity DbEntity, Item Item = null, Db.IDbContext ctx = null) where EntityT : EntityBase
        {
            if (DbEntity == null)
            {
                throw new ArgumentNullException(nameof(DbEntity));
            }

            int EntityTypeID = DbEntity.EntityTypeID;
            EntityType EntityType = (EntityType)EntityTypeID;

            if (typeof(EntityT) != EntityType.Type)
            {
                throw new InvalidOperationException("The entity type from the database object does not match the type parameter.");
            }

            return (EntityT)Activator.CreateInstance(EntityType.Type, new object[] { Item, DbEntity, ctx });
        }

        /// <summary>
        /// Generates an entity from database object.
        /// </summary>
        /// <param name="DbEntity">The entity from the database. Can be null.</param>
        /// <param name="Item">An optional parent Item.</param>
        /// <param name="ctx">Database context (optional)</param>
        /// <returns>The generated Entity object.</returns>
        public static EntityBase GenerateEntityFromDbObject(Db.Entity DbEntity, Item Item = null, Db.IDbContext ctx = null)
        {
            if (DbEntity == null)
            {
                throw new ArgumentNullException(nameof(DbEntity));
            }

            int EntityTypeID = DbEntity.EntityTypeID;
            EntityType EntityType = (EntityType)EntityTypeID;

            if (!typeof(EntityBase).IsAssignableFrom(EntityType.Type))
            {
                throw new InvalidOperationException("Cannot create entity from this type.");
            }

            return (EntityBase)Activator.CreateInstance(EntityType.Type, new object[] { Item, DbEntity, ctx });
        }

        /// <summary>
        /// Generates a new entity from the given EntityType
        /// </summary>
        /// <param name="Type">The EntityType</param>
        /// <param name="Item">An optional Item to bind to the Entity</param>
        /// <param name="ctx">Database context (optional)</param>
        /// <returns>The newly created Entity</returns>
        public static EntityBase GetEntityByType(EntityType Type, Item Item = null, Db.IDbContext ctx = null)
        {
            if (Type == null)
            {
                throw new ArgumentNullException(nameof(Type));
            }
            return GetEntityBySystemType(Type.Type, Item, ctx);
        }

        /// <summary>
        /// Sets the OriginalValue cache so long as the value is pure
        /// </summary>
        protected void RefreshOriginalValues()
        {
            if (!this.Dirty)
            {
                this.OriginalEffectiveDate = this.EffectiveDate;
                this.OriginalEndEffectiveDate = this.EndEffectiveDate;
            }
        }
    }
}
