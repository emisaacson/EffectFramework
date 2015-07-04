using System;
using EffectFramework.Core.Models.Entities;

namespace EffectFramework.Core.Models.Fields
{

    [Serializable]
    public class FieldDecimal : FieldBase, IField
    {
        public decimal? Value
        {
            get
            {
                return this.ValueDecimal;
            }
            set
            {
                decimal? _Value = value;
                if (!this.ValueDecimal.Equals(_Value))
                {
                    this.Dirty = true;
                    this.ValueDecimal = _Value;
                }
            }
        }

        object IField.Value
        {
            get
            {
                return this.ValueDecimal;
            }

            set
            {
                if (value != null && !typeof(decimal?).IsAssignableFrom(value.GetType()))
                {
                    throw new InvalidCastException("Must assign a decimal key to a decimal field.");
                }
                decimal? _Value = (decimal?)value;
                if (!this.ValueDecimal.Equals(_Value))
                {
                    this.Dirty = true;
                    this.ValueDecimal = _Value;
                }
            }
        }

        public FieldTypeMetaDecimal MetaLookup
        {
            get
            {
                return (FieldTypeMetaDecimal)Meta;
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
                if (!(value is FieldTypeMetaDecimal))
                {
                    Log.Error("Must assign a FieldTypeMetaDecimal to a decimal field. Value type: {0}, Field ID: {1}", value.GetType().Name, FieldID);
                    throw new InvalidCastException("Must assign a FieldTypeMetaDecimal to a lookup field.");
                }
                _Meta = (FieldTypeMetaDecimal)value;
            }
        }


        public object DereferencedValue
        {
            get
            {
                return this.ValueDecimal;
            }
        }

        public decimal? OriginalValue
        {
            get
            {
                return this.OriginalValueDecimal;
            }
        }

        object IField.OriginalValue
        {
            get
            {
                return this.OriginalValueDecimal;
            }
        }

        public object OriginalDereferencedValue
        {
            get
            {
                return this.OriginalValueDecimal;
            }
        }

        public bool ValueEquals(object Value)
        {
            if (Value == null)
            {
                return this.Value == null;
            }
            else if (Value is string)
            {
                decimal PossibleDecimalParsed;
                if (decimal.TryParse((string)Value, out PossibleDecimalParsed))
                {
                    return this.Value.HasValue && this.Value.Value == PossibleDecimalParsed;
                }
                else
                {
                    return false;
                }
            }
            else if (Value is decimal)
            {
                return this.Value.Value == (decimal)Value;
            }
            return false;
        }

        public FieldDecimal()
            : base()
        { }

        public FieldDecimal(FieldType Type)
            : this(Type, null, null)
        {

        }

        public FieldDecimal(FieldType Type, EntityBase Entity)
            : this(Type, null, Entity)
        {

        }

        public FieldDecimal(FieldType Type, FieldBase Base, EntityBase Entity)
            : base(Type, Base, Entity)
        {
            if (Type.DataType != DataType.Decimal)
            {
                throw new ArgumentOutOfRangeException("Cannot create a decimal field from a non-decimal type.");
            }
        }
    }
}
