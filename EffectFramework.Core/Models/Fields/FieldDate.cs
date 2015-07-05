using System;
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


        protected override IFieldTypeMeta DefaultMeta
        {
            get
            {
                return new FieldTypeMetaDate(false, null, null);
            }
        }

        public FieldTypeMetaDate MetaDate
        {
            get
            {
                return (FieldTypeMetaDate)Meta;
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

        public bool ValueEquals(object Value)
        {
            if (Value == null)
            {
                return !this.Value.HasValue;
            }
            else if (Value is DateTime)
            {
                return this.Value.HasValue && this.Value.Value == (DateTime)Value;
            }
            else if (Value is string)
            {
                return this.Value.HasValue && this.Value.Value == Convert.ToDateTime((string)Value);
            }
            return false;
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

        public FieldDate(FieldType Type, FieldBase Base, EntityBase Entity, Db.IDbContext ctx = null)
            : base(Type, Base, Entity, ctx)
        {
            if (Type.DataType != DataType.Date)
            {
                throw new ArgumentOutOfRangeException("Cannot create a date field from a non-date type.");
            }
        }
    }
}