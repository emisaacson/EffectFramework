using System;
using EffectFramework.Core.Services;
using EffectFramework.Core.Models.Entities;

namespace EffectFramework.Core.Models.Fields
{
    public class FieldDate : FieldBase, IField
    {
        public string Name { get; private set; }
        public DateTime? Value
        {
            get
            {
                return this.ValueDate;
            }

            set
            {
                DateTime? _Value = (DateTime?)value;
                if (_Value.HasValue && _Value.Value == default(DateTime))
                {
                    _Value = null;
                }
                if (!this.ValueDate.HasValue || this.ValueDate.Value != _Value)
                {
                    this.Dirty = true;
                    this.ValueDate = _Value;
                }
            }
        }

        object IField.Value
        {
            get
            {
                return this.ValueDate;
            }

            set
            {
                if (value != null && !typeof(DateTime?).IsAssignableFrom(value.GetType()))
                {
                    throw new InvalidCastException("Must assign a datetime to a date field.");
                }
                DateTime? _Value = (DateTime?)value;
                if (_Value.HasValue && _Value.Value == default(DateTime))
                {
                    _Value = null;
                }
                if (!this.ValueDate.HasValue || this.ValueDate.Value != _Value)
                {
                    this.Dirty = true;
                    this.ValueDate = _Value;
                }
            }
        }

        public object DereferencedValue
        {
            get
            {
                return Value;
            }
        }

        public DateTime? OriginalValue
        {
            get
            {
                return this.OriginalValueDate;
            }
        }

        object IField.OriginalValue
        {
            get
            {
                return this.OriginalValueDate;
            }
        }

        public object OriginalDereferencedValue
        {
            get
            {
                return this.OriginalValueDate;
            }
        }

        public FieldDate(IPersistenceService PersistenceService)
            : base(PersistenceService)
        { }

        public FieldDate(FieldType Type, IPersistenceService PersistenceService)
            : this(Type, null, null, PersistenceService)
        { }

        public FieldDate(FieldType Type, EntityBase Entity, IPersistenceService PersistenceService)
            : this(Type, null, Entity, PersistenceService)
        {

        }

        public FieldDate(FieldType Type, FieldBase Base, EntityBase Entity, IPersistenceService PersistenceService)
            : base(PersistenceService)
        {
            if (Type.DataType != DataType.Date)
            {
                throw new ArgumentOutOfRangeException("Cannot create a date field from a non-date type.");
            }
            this.Type = Type;
            this.Name = Type.Name;
            this.Entity = Entity;

            if (Base != null)
            {
                LoadUpValues(Base);
            }
        }
    }
}