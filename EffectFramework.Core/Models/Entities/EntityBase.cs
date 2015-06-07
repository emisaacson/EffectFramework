using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EffectFramework.Core.Models.Fields;
using EffectFramework.Core.Services;
using EffectFramework.Core.Models.Annotations;
using Ninject;

namespace EffectFramework.Core.Models.Entities
{
    public abstract class EntityBase
    {
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

        protected IPersistenceService _PersistenceService = null;
        public IPersistenceService PersistenceService {
            get
            {
                return _PersistenceService;
            }
            set
            {
                if (_PersistenceService == null)
                {
                    _PersistenceService = value;
                    WireUpFields();
                }
                else
                {
                    throw new InvalidOperationException("Cannot set the persistence service more than once.");
                }
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

        public EntityBase()
        {
            Log.Trace("Creating new EntityBase object. Entity Type: {0}", this.Type.Name);

            this.Dirty = true;
            this.IsDeleted = false;
            this.FlagForRemoval = false;
        }

        public EntityBase(IPersistenceService PersistenceService)
        {
            Log.Trace("Creating new EntityBase with PersistenceService. Entity Type: {0}", this.Type.Name);

            this._PersistenceService = PersistenceService;
            this.Dirty = true;
            this.IsDeleted = false;
            this.FlagForRemoval = false;
            WireUpFields();
        }

        public void LoadUpEntity(Db.Entity DbEntity)
        {
            Log.Trace("Loading up entity fields from database. EntityID: {0}", DbEntity.EntityID);

            this.EntityID = DbEntity.EntityID;
            this.ItemID = DbEntity.ItemID;
            this.Guid = DbEntity.Guid;
            this._EffectiveDate = DbEntity.EffectiveDate;
            this._EndEffectiveDate = DbEntity.EndEffectiveDate;
            this.IsDeleted = DbEntity.IsDeleted;

            var FieldObjects = GetAllEntityFields();

            foreach (var FieldObject in FieldObjects)
            {
                FieldObject.FillFromDatabase(this, FieldObject);
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

        public void CopyValuesFrom(EntityBase OtherEntity)
        {
            if (OtherEntity == null)
            {
                Log.Warn("Trying to copy values from a null entity. EntityID: {0}", this.EntityID.HasValue ? this.EntityID.Value.ToString() : "null");
                throw new ArgumentNullException();
            }
            if (OtherEntity.Type != this.Type)
            {
                Log.Warn("Trying to copy values from a different entity type. EntityID: {0}, Other Entity ID: {1}, Entity Type: {3}, Other Entity Type: {4}",
                    this.EntityID.HasValue ? this.EntityID.Value.ToString() : "null",
                    OtherEntity.EntityID.HasValue ? OtherEntity.EntityID.Value.ToString() : "null",
                    this.Type.Name, OtherEntity.Type.Name);

                throw new InvalidOperationException("Must copy values from an entity of the same type.");
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
        /// Persists the entity to the database by saving its own properties
        /// then each of its fields.
        /// </summary>
        /// <param name="ctx">An optional database context. One will
        /// be created if not provided.</param>
        public void PersistToDatabase(Db.IDbContext ctx = null)
        {
            PersistenceService.RecordAudit(this, null, null, ctx);

            var Identity = PersistenceService.SaveSingleEntity(this, ctx);
            this.Guid = Identity.ObjectGuid;
            this.EntityID = Identity.ObjectID;

            var FieldObjects = GetAllEntityFields();

            foreach (var FieldObject in FieldObjects)
            {
                FieldObject.PersistToDatabase(this, ctx);
            }

            this.Dirty = false;
            RefreshOriginalValues();

            if (this.IsDeleted)
            {
                this.FlagForRemoval = true;
            }
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
                throw new ArgumentNullException();
            }
            if (OtherEntity.Type != this.Type)
            {
                throw new InvalidOperationException("Cannot compare entities of different types.");
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

        public void PersistToDatabase(Item Item, Db.IDbContext ctx = null)
        {
            if (Item == null)
            {
                throw new ArgumentNullException();
            }

            PersistenceService.RecordAudit(this, null, null, ctx);

            var Identity = PersistenceService.SaveSingleEntity(Item, this, ctx);
            this.Guid = Identity.ObjectGuid;
            this.EntityID = Identity.ObjectID;

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

            var FieldObjects = GetAllEntityFields();

            foreach (var FieldObject in FieldObjects)
            {
                FieldObject.PersistToDatabase(this, ctx);
            }

            this.Dirty = false;
            RefreshOriginalValues();

            foreach (var PossiblePreviousEntity in PossiblePreviousEntities)
            {
                if (PossiblePreviousEntity != null)
                {
                    if (this.IsIdenticalTo(PossiblePreviousEntity))
                    {
                        PossiblePreviousEntity.EndEffectiveDate = this.EndEffectiveDate;
                        PossiblePreviousEntity.PersistToDatabase(Item, ctx);
                        this.Seppuku(ctx);
                    }
                }
            }
        }

        public static EntityBase GetEntityBySystemType(Type SystemType)
        {
            if (SystemType == null)
            {
                throw new ArgumentNullException();
            }
            if (!typeof(EntityBase).IsAssignableFrom(SystemType))
            {
                throw new ArgumentOutOfRangeException("Cannot create an entity from a type that isn't a subclass of EntityBase.");
            }

            using (IKernel Kernel = new StandardKernel(new Configure()))
            {
                return (EntityBase)Kernel.Get(SystemType);
            }
        }

        public static EntityBase GetEntityByType(EntityType Type)
        {
            if (Type == null)
            {
                throw new ArgumentNullException();
            }
            return GetEntityBySystemType(Type.Type);
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
