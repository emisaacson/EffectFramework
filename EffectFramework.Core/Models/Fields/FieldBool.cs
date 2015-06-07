using System;
using EffectFramework.Core.Services;
using EffectFramework.Core.Models.Entities;

namespace EffectFramework.Core.Models.Fields
{
    public class FieldBool : FieldBase, IField
    {
        public string Name { get; private set; }
        public bool? Value
        {
            get
            {
                return this.ValueBool;
            }
            set
            {
                this.Dirty = true;
                this.ValueBool = value;
            }
        }

        object IField.Value
        {
            get
            {
                return this.ValueBool;
            }

            set
            {
                if (value != null && !typeof(bool?).IsAssignableFrom(value.GetType()))
                {
                    throw new InvalidCastException("Must assign a boolean to a boolean field.");
                }
                if (this.ValueBool != (bool?)value)
                {
                    this.Dirty = true;
                    this.ValueBool = (bool?)value;
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

        public FieldBool(IPersistenceService PersistenceService)
            : base(PersistenceService)
        { }

        public FieldBool(FieldType Type, IPersistenceService PersistenceService)
            : this(Type, null, null, PersistenceService)
        {

        }

        public FieldBool(FieldType Type, EntityBase Entity, IPersistenceService PersistenceService)
            : this(Type, null, Entity, PersistenceService)
        {

        }

        public FieldBool(FieldType Type, FieldBase Base, EntityBase Entity, IPersistenceService PersistenceService)
            : base(PersistenceService)
        {
            if (Type.DataType != DataType.Boolean)
            {
                throw new ArgumentOutOfRangeException("Cannot create a boolean field from a non-boolean type.");
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
