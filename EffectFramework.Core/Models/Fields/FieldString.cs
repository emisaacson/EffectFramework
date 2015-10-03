using System;
using EffectFramework.Core.Models.Entities;
using System.Collections.Generic;

namespace EffectFramework.Core.Models.Fields
{
    [Serializable]
    public class FieldString : FieldBase, IField
    {
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
                if (value != null && !typeof(String).IsAssignableFrom(value.GetType())) {
                    throw new InvalidCastException("Must assign a string to a string field.");
                }
                if (this.ValueString != (string)value)
                {
                    this.Dirty = true;
                    this.ValueString = (string)value;
                }
            }
        }


        protected override IFieldTypeMeta DefaultMeta
        {
            get
            {
                return new FieldTypeMetaText(false, null);
            }
        }
        public FieldTypeMetaText MetaText
        {
            get
            {
                return (FieldTypeMetaText)Meta;
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
                if (!(value is FieldTypeMetaText))
                {
                    Log.Error("Must assign a FieldTypeMetaText to a text field. Value type: {0}, Field ID: {1}", value.GetType().Name, FieldID);
                    throw new InvalidCastException("Must assign a FieldTypeMetaText to a text field.");
                }
                _Meta = (FieldTypeMetaText)value;
            }
        }

        protected override ValidationSummary ValidationHook()
        {
            var Errors = new List<ValidationResult>();

            if (Meta.HasRegex && !string.IsNullOrWhiteSpace(this.Value) && !Meta.TextRegex.IsMatch(this.Value))
            {
                Errors.Add(new ValidationResult(this, string.Format(Strings.Field_Does_Not_Validate, this.Entity.Type.Name, this.Name)));
            }
            return new ValidationSummary(Errors);
        }

        public object DereferencedValue
        {
            get
            {
                return Value;
            }
        }

        public string OriginalValue
        {
            get
            {
                return this.OriginalValueString;
            }
        }

        object IField.OriginalValue
        {
            get
            {
                return this.OriginalValueString;
            }
        }

        public object OriginalDereferencedValue
        {
            get
            {
                return this.OriginalValueString;
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
                return (string)this.Value == (string)Value;
            }
            return false;
        }


        public FieldString()
            : base()
        { }

        public FieldString(FieldType Type)
            : this(Type, null, null)
        {

        }

        public FieldString(FieldType Type, EntityBase Entity)
            : this(Type, null, Entity)
        {

        }

        public FieldString(FieldType Type, FieldBase Base, EntityBase Entity, Db.IDbContext ctx = null)
            : base(Type, Base, Entity, ctx)
        {
            if (Type.DataType != DataType.Text)
            {
                throw new ArgumentOutOfRangeException("Cannot create a string field from a non-string type.");
            }
        }
    }
}
