using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EffectFramework.Core.Models.Fields;
using EffectFramework.Core.Services;

namespace EffectFramework.Core.Models.Entities
{
    public abstract class EntityBase
    {
        public abstract EntityType Type { get; }
        public int? EntityID { get; protected set; }
        public int? ItemID { get; protected set; }
        public Guid Guid { get; protected set; }
        public bool Dirty { get; protected set; }
        public Item Item { get; internal set; }
        internal bool FlagForRemoval { get; set; }

        private DateTime _EffectiveDate;
        public DateTime EffectiveDate {
            get {
                return this._EffectiveDate;
            }
            set
            {
                if (this._EffectiveDate != value)
                {
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

        protected abstract void WireUpFields();

        public EntityBase()
        {
            this.Dirty = true;
            this.FlagForRemoval = false;
        }

        public EntityBase(IPersistenceService PersistenceService)
        {
            this._PersistenceService = PersistenceService;
            this.Dirty = true;
            this.FlagForRemoval = false;
            WireUpFields();
        }

        public void LoadUpEntity(Db.Entity DbEntity)
        {
            this.EntityID = DbEntity.EntityID;
            this.ItemID = DbEntity.ItemID;
            this.Guid = DbEntity.Guid;
            this._EffectiveDate = DbEntity.EffectiveDate;
            this._EndEffectiveDate = DbEntity.EndEffectiveDate;

            var FieldObjects = GetAllEntityFieldProperties();

            foreach (var FieldObject in FieldObjects)
            {
                FieldObject.FillFromDatabase(this, FieldObject);
            }

            this.Dirty = false;
        }

        private void Seppuku(Db.IDbContext ctx = null)
        {
            if (this.EntityID.HasValue && !this.FlagForRemoval)
            {
                PersistenceService.SaveAndDeleteSingleEntity(this, ctx);
            }
            this.FlagForRemoval = true;
        }

        private List<FieldBase> GetAllEntityFieldProperties()
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

        public void PersistToDatabase(Db.IDbContext ctx = null)
        {
            var Identity = PersistenceService.SaveSingleEntity(this, ctx);
            this.Guid = Identity.ObjectGuid;
            this.EntityID = Identity.ObjectID;

            var FieldObjects = GetAllEntityFieldProperties();

            foreach (var FieldObject in FieldObjects)
            {
                FieldObject.PersistToDatabase(this, ctx);
            }

            this.Dirty = false;
        }

        public void PersistToDatabase(Item Item, Db.IDbContext ctx = null)
        {
            if (Item == null)
            {
                throw new ArgumentNullException();
            }

            var Identity = PersistenceService.SaveSingleEntity(Item, this, ctx);
            this.Guid = Identity.ObjectGuid;
            this.EntityID = Identity.ObjectID;

            var PossiblePreviousEntity = Item.AllEntities
                .Where(e => e.Type == this.Type &&
                            e.EndEffectiveDate.HasValue &&
                            e.EndEffectiveDate.Value == this.EffectiveDate &&
                            // This check is necessary to prevent two entities from each
                            // being possible previous entities of each other, which would
                            // cause infinite recursion.
                            (!this.EndEffectiveDate.HasValue || this.EndEffectiveDate.Value != e.EffectiveDate) &&
                            // Exclude the current entity
                            e != this).SingleOrDefault();

            var FieldObjects = GetAllEntityFieldProperties();

            foreach (var FieldObject in FieldObjects)
            {
                FieldObject.PersistToDatabase(this, ctx);
            }

            this.Dirty = false;

            if (PossiblePreviousEntity != null)
            {
                var PreviousEntityFieldObjects = PossiblePreviousEntity.GetAllEntityFieldProperties();
                bool AreIdentical = true;
                foreach (var PreviousEntityField in PreviousEntityFieldObjects)
                {
                    var CurrentEntityField = FieldObjects.Where(f => f.Type == PreviousEntityField.Type).Single();
                    if (!(((IField)CurrentEntityField).Value == null && ((IField)PreviousEntityField).Value == null) && // If both null, they are identical. stop
                        ((((IField)CurrentEntityField).Value == null && ((IField)PreviousEntityField).Value != null) || // If a is null and b is not, not identical
                         (((IField)PreviousEntityField).Value == null && ((IField)CurrentEntityField).Value != null) || // If b is null and a is not, not identical
                         !((IField)CurrentEntityField).Value.Equals(((IField)PreviousEntityField).Value)))              // Neither are null, use the default comparer
                    {
                        AreIdentical = false;
                        break;
                    }
                }
                if (AreIdentical)
                {
                    PossiblePreviousEntity.EndEffectiveDate = this.EndEffectiveDate;
                    PossiblePreviousEntity.PersistToDatabase(Item, ctx);
                    this.Seppuku(ctx);
                }
            }

        }
    }
}
