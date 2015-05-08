using System;
using EffectFramework.Core.Services;

namespace EffectFramework.Core.Models.Fields
{
    public class FieldString : FieldBase, IField
    {
        public string Name { get; private set; }
        public string Value
        {
            get
            {
                return this.ValueString;
            }
            set
            {
                this.Dirty = true;
                this.ValueString = value;
            }
        }

        object IField.Value
        {
            get
            {
                return this.ValueString;
            }

            set
            {
                if (!typeof(String).IsAssignableFrom(value.GetType())) {
                    throw new InvalidCastException("Must assign a string to a string field.");
                }
                if (this.ValueString != (string)value)
                {
                    this.Dirty = true;
                    this.ValueString = (string)value;
                }
            }
        }

        public FieldString(IPersistenceService PersistenceService)
            : base(PersistenceService)
        { }

        public FieldString(FieldType Type, IPersistenceService PersistenceService)
            : this(Type, null, PersistenceService)
        {

        }

        public FieldString(FieldType Type, FieldBase Base, IPersistenceService PersistenceService)
            : base(PersistenceService)
        {
            if (Type.DataType != DataType.Text)
            {
                throw new ArgumentOutOfRangeException("Cannot create a string field from a non-string type.");
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
