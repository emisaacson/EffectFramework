using System;
using System.Collections.Generic;
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
        }

        public EntityBase(IPersistenceService PersistenceService)
        {
            this._PersistenceService = PersistenceService;
            this.Dirty = true;
            WireUpFields();
        }

        public void LoadUpEntity(Db.Entity DbEntity)
        {
            this.EntityID = DbEntity.EntityID;
            this.ItemID = DbEntity.ItemID;
            this.Guid = DbEntity.Guid;
            this.Dirty = false;

            var FieldObjects = GetAllEntityFieldProperties();

            foreach (var FieldObject in FieldObjects)
            {
                FieldObject.FillFromDatabase(this, FieldObject);
            }
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

        public void PersistEntityToDatabase(Db.IDbContext ctx = null)
        {
            var Identity = PersistenceService.SaveEntity(this, ctx);
            this.Guid = Identity.ObjectGuid;
            this.EntityID = Identity.ObjectID;

            var FieldObjects = GetAllEntityFieldProperties();

            foreach (var FieldObject in FieldObjects)
            {
                FieldObject.PersistToDatabase(this, ctx);
            }
        }
    }
}
