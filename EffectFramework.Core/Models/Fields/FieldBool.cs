using System;
using EffectFramework.Core.Models.Entities;

namespace EffectFramework.Core.Models.Fields
{
    [Serializable]
    public class FieldBool : FieldBase, IField
    {
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
                    Log.Error("Must assign a boolean to a boolean field. Value type: {0}, FieldID: {1}", value.GetType().Name, FieldID);
                    throw new InvalidCastException("Must assign a boolean to a boolean field.");
                }
                if (this.ValueBool != (bool?)value)
                {
                    this.Dirty = true;
                    this.ValueBool = (bool?)value;
                }
            }
        }

        public FieldTypeMetaBasic MetaBool
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
                return _Meta;
            }
            protected set
            {
                if (!(value is FieldTypeMetaBasic))
                {
                    Log.Error("Must assign a FieldTypeMetaBasic to a boolean field. Value type: {0}, Field ID: {1}", value.GetType().Name, FieldID);
                    throw new InvalidCastException("Must assign a FieldTypeMetaBasic to a boolean field.");
                }
                _Meta = (FieldTypeMetaBasic)value;
            }
        }

        public object DereferencedValue
        {
            get
            {
                return Value;
            }
        }

        public bool? OriginalValue
        {
            get
            {
                return this.OriginalValueBool;
            }
        }

        object IField.OriginalValue
        {
            get
            {
                return this.OriginalValueBool;
            }
        }

        public object OriginalDereferencedValue
        {
            get
            {
                return this.OriginalValueBool;
            }
        }

        public bool ValueEquals(object Value)
        {
            if (Value == null)
            {
                return !this.Value.HasValue;
            }
            else if (Value is bool)
            {
                return this.Value.HasValue && this.Value.Value == (bool)Value;
            }
            else if (Value is string)
            {
                return this.Value.HasValue && this.Value.Value == Convert.ToBoolean((string)Value);
            }
            return false;
        }

        public FieldBool()
            : base()
        { }

        public FieldBool(FieldType Type)
            : this(Type, null, null)
        {

        }

        public FieldBool(FieldType Type, EntityBase Entity)
            : this(Type, null, Entity)
        {

        }

        public FieldBool(FieldType Type, FieldBase Base, EntityBase Entity)
            : base(Type, Base, Entity)
        {
            if (Type.DataType != DataType.Boolean)
            {
                throw new ArgumentOutOfRangeException("Cannot create a boolean field from a non-boolean type.");
            }
        }
    }
}
