using System;
using EffectFramework.Core.Services;
using EffectFramework.Core.Models.Entities;

namespace EffectFramework.Core.Models.Fields
{
    [Serializable]
    public class FieldDate : FieldBase, IField
    {
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
                    Log.Error("Must assign a datetime to a date field. Value type: {0}, FieldID: {1}", value.GetType().Name, FieldID);
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

        private FieldTypeMetaDate _Meta;
        private FieldTypeMetaDate _DefaultMeta = new FieldTypeMetaDate(false, null, null);
        public FieldTypeMetaDate MetaDate
        {
            get
            {
                if (_Meta == null)
                {
                    TryLoadFieldMeta();
                }
                if (_Meta /*still*/ == null)
                {
                    return _DefaultMeta;
                }
                return _Meta;
            }
        }
        public override IFieldTypeMeta Meta
        {
            get
            {
                return MetaDate;
            }
            protected set
            {
                if (!(value is FieldTypeMetaDate))
                {
                    Log.Error("Must assign a FieldTypeMetaDate to a date field. Value type: {0}, Field ID: {1}", value.GetType().Name, FieldID);
                    throw new InvalidCastException("Must assign a FieldTypeMetaDate to a date field.");
                }
                _Meta = (FieldTypeMetaDate)value;
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

        public FieldDate()
            : base()
        { }

        public FieldDate(FieldType Type)
            : this(Type, null, null)
        { }

        public FieldDate(FieldType Type, EntityBase Entity)
            : this(Type, null, Entity)
        {

        }

        public FieldDate(FieldType Type, FieldBase Base, EntityBase Entity)
            : base(Type, Base, Entity)
        {
            if (Type.DataType != DataType.Date)
            {
                throw new ArgumentOutOfRangeException("Cannot create a date field from a non-date type.");
            }
        }
    }
}