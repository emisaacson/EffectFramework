﻿using System;
using EffectFramework.Core.Services;
using EffectFramework.Core.Models.Entities;

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

        private FieldTypeMetaText _Meta;
        private FieldTypeMetaText _DefaultMeta = new FieldTypeMetaText(false, null);
        public FieldTypeMetaText MetaText
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
                return MetaText;
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

        public FieldString(FieldType Type, FieldBase Base, EntityBase Entity)
            : base(Type, Base, Entity)
        {
            if (Type.DataType != DataType.Text)
            {
                throw new ArgumentOutOfRangeException("Cannot create a string field from a non-string type.");
            }
        }
    }
}
