using System;
using EffectFramework.Core.Services;

namespace EffectFramework.Core.Models.Fields
{
    public class FieldBinary : FieldBase, IField
    {
        public string Name { get; private set; }
        public byte[] Value
        {
            get
            {
                return this.ValueBinary;
            }
            set
            {
                this.Dirty = true;
                this.ValueBinary = value;
            }
        }

        object IField.Value
        {
            get
            {
                return this.ValueBinary;
            }

            set
            {
                if (!typeof(byte[]).IsAssignableFrom(value.GetType()))
                {
                    throw new InvalidCastException("Must assign a byte array to a binary field.");
                }
                if (this.ValueBinary != (byte[])value)
                {
                    this.Dirty = true;
                    this.ValueBinary = (byte[])value;
                }
            }
        }

        public FieldBinary(IPersistenceService PersistenceService)
            : base(PersistenceService)
        { }

        public FieldBinary(FieldType Type, IPersistenceService PersistenceService)
            : this(Type, null, PersistenceService)
        {

        }

        public FieldBinary(FieldType Type, FieldBase Base, IPersistenceService PersistenceService)
            : base(PersistenceService)
        {
            if (Type.DataType != DataType.Binary)
            {
                throw new ArgumentOutOfRangeException("Cannot create a binary field from a non-binary type.");
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
