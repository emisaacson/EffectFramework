using System;
using System.Reflection;
using HRMS.Core.Models.Fields;
using HRMS.Core.Services;

namespace HRMS.Core.Models.Entities
{
    public abstract class EntityBase
    {
        public abstract EntityType Type { get; }
        public int? EntityID { get; protected set; }
        public int? EmployeeID { get; protected set; }
        public Guid Guid { get; protected set; }
        public bool Dirty { get; protected set; }

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
            this.EmployeeID = DbEntity.EmployeeID;
            this.Guid = DbEntity.Guid;
            this.Dirty = false;

            Type EntityType = this.GetType();

            PropertyInfo[] AllPublicProperties = EntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo Property in AllPublicProperties)
            {
                if (typeof(FieldBase).IsAssignableFrom(Property.PropertyType))
                {
                    FieldBase FieldObject = (FieldBase)Property.GetValue(this);
                    FieldObject.FillFromDatabase(this, FieldObject);
                }
            }
        }
    }
}
