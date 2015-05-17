using System;
using EffectFramework.Core.Services;

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
                if (this.ValueDate != value)
                {
                    this.Dirty = true;
                    this.ValueDate = value;
                }
            }
        }

        object IField.Value
        {
            get
            {
                if (this.ValueDate.HasValue)
                {
                    return this.ValueDate.Value;
                }
                return null;
            }

            set
            {
                if (value != null && !typeof(DateTime?).IsAssignableFrom(value.GetType()))
                {
                    throw new InvalidCastException("Must assign a datetime to a date field.");
                }
                if (!this.ValueDate.HasValue || this.ValueDate.Value != (DateTime?)value)
                {
                    this.Dirty = true;
                    this.ValueDate = (DateTime?)value;
                }
            }
        }

        public FieldDate(IPersistenceService PersistenceService)
            : base(PersistenceService)
        { }

        public FieldDate(FieldType Type, IPersistenceService PersistenceService)
            : this(Type, null, PersistenceService)
        {

        }

        public FieldDate(FieldType Type, FieldBase Base, IPersistenceService PersistenceService)
            : base(PersistenceService)
        {
            if (Type.DataType != DataType.Date)
            {
                throw new ArgumentOutOfRangeException("Cannot create a date field from a non-date type.");
            }
            this.Type = Type;
            this.Name = Type.Name;

            if (Base != null)
            {
                LoadUpValues(Base);
            }
        }
    }
}