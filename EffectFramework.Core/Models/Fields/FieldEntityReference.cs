using EffectFramework.Core.Models.Entities;
using System;

namespace EffectFramework.Core.Models.Fields
{
    [Serializable]
    public class FieldEntityReference : FieldBase, IField
    {
        public long? Value
        {
            get
            {
                return this.ValueEntityReference;
            }
            set
            {
                long? _Value = value;
                if (_Value.HasValue && _Value.Value == default(long))
                {
                    _Value = null;
                }
                if (!this.ValueEntityReference.Equals(_Value))
                {
                    this.Dirty = true;
                    this.ValueEntityReference = _Value;
                }
            }
        }

        object IField.Value
        {
            get
            {
                return this.ValueEntityReference;
            }

            set
            {
                if (value != null && !typeof(long?).IsAssignableFrom(value.GetType()))
                {
                    throw new InvalidCastException("Must assign a long key to an entity reference field.");
                }
                long? _Value = (long?)value;
                if (_Value.HasValue && _Value.Value == default(long))
                {
                    _Value = null;
                }
                if (!this.ValueEntityReference.Equals(_Value))
                {
                    this.Dirty = true;
                    this.ValueEntityReference = _Value;
                }
            }
        }

        public FieldTypeMetaBasic MetaLookup
        {
            get
            {
                return (FieldTypeMetaBasic)Meta;
            }
        }
        public override IFieldTypeMeta Meta
        {
            get
            {
                return base.Meta;
            }
            protected set
            {
                if (!(value is FieldTypeMetaBasic))
                {
                    Log.Error("Must assign a FieldTypeMetaBasic to an entity reference field. Value type: {0}, Field ID: {1}", value.GetType().Name, FieldID);
                    throw new InvalidCastException("Must assign a FieldTypeMetaBasic to an entity reference field.");
                }
                _Meta = (FieldTypeMetaBasic)value;
            }
        }

        [NonSerialized]
        private EntityBase _DereferencedValue;
        public object DereferencedValue
        {
            get
            {
                if (_DereferencedValue == null && this.Value.HasValue)
                {
                    var Deref = PersistenceService.RetreiveSingleEntityOrDefault(this.Value.Value);
                    if (Deref.TenantID != this.TenantID)
                    {
                        throw new Exceptions.FatalException("Data error.");
                    }
                    _DereferencedValue = Deref;
                }
                return _DereferencedValue;
            }
        }

        public long? OriginalValue
        {
            get
            {
                return this.OriginalValueEntityReference;
            }
        }

        object IField.OriginalValue
        {
            get
            {
                return this.OriginalValueEntityReference;
            }
        }

        [NonSerialized]
        private EntityBase _OriginalDereferencedValue;
        public object OriginalDereferencedValue
        {
            get
            {
                if (_OriginalDereferencedValue == null && this.OriginalValue.HasValue)
                {
                    var Deref = PersistenceService.RetreiveSingleEntityOrDefault(this.OriginalValue.Value);
                    if (Deref.TenantID != this.TenantID) {
                        throw new Exceptions.FatalException("Data error.");
                    }
                    _OriginalDereferencedValue = Deref;
                }
                return _OriginalDereferencedValue;
            }
        }

        public bool ValueEquals(object Value)
        {
            if (Value == null)
            {
                return this.Value == null;
            }
            else if (Value is EntityBase)
            {
                return ((EntityBase)Value).EntityID.HasValue && this.Value.Value == ((EntityBase)Value).EntityID.Value;
            }
            else if (Value is long || Value is int)
            {
                return this.Value.Value == (long)Value;
            }
            return false;
        }

        public FieldEntityReference()
            : base()
        { }

        public FieldEntityReference(FieldType Type)
            : this(Type, null, null)
        {

        }

        public FieldEntityReference(FieldType Type, EntityBase Entity)
            : this(Type, null, Entity)
        {

        }

        public FieldEntityReference(FieldType Type, FieldBase Base, EntityBase Entity, Db.IDbContext ctx = null)
            : base(Type, Base, Entity, ctx)
        {
            if (Type.DataType != DataType.EntityReference)
            {
                throw new ArgumentOutOfRangeException("Cannot create an entity reference field from a non-entity-reference type.");
            }
        }
    }
}
