﻿using System;
using System.Linq;
using EffectFramework.Core.Models.Entities;

namespace EffectFramework.Core.Models.Fields
{
    [Serializable]
    public class FieldLookup : FieldBase, IField
    {
        public int? Value
        {
            get
            {
                return this.ValueLookup;
            }
            set
            {
                int? _Value = value;
                if (_Value.HasValue && _Value.Value == default(int))
                {
                    _Value = null;
                }
                if (!this.ValueLookup.Equals(_Value))
                {
                    this.Dirty = true;
                    this.ValueLookup = _Value;
                }
            }
        }

        object IField.Value
        {
            get
            {
                return this.ValueLookup;
            }

            set
            {
                if (value != null && !typeof(int?).IsAssignableFrom(value.GetType()))
                {
                    throw new InvalidCastException("Must assign a int key to a lookup field.");
                }
                int? _Value = (int?)value;
                if (_Value.HasValue && _Value.Value == default(int))
                {
                    _Value = null;
                }
                if (!this.ValueLookup.Equals(_Value))
                {
                    this.Dirty = true;
                    this.ValueLookup = _Value;
                }
            }
        }

        public FieldTypeMetaBasic MetaLookup
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
                return base.Meta;
            }
            protected set
            {
                if (!(value is FieldTypeMetaBasic))
                {
                    Log.Error("Must assign a FieldTypeMetaBasic to a lookup field. Value type: {0}, Field ID: {1}", value.GetType().Name, FieldID);
                    throw new InvalidCastException("Must assign a FieldTypeMetaBasic to a lookup field.");
                }
                _Meta = (FieldTypeMetaBasic)value;
            }
        }


        public object DereferencedValue
        {
            get
            {
                if (this.Value.HasValue)
                {
                    var Deref = Choices.Choices.Where(c => c.ID == this.Value.Value).FirstOrDefault();
                    if (Deref != null)
                    {
                        return Deref.Value;
                    }
                    return null;
                }
                return null;
            }
        }

        public int? OriginalValue
        {
            get
            {
                return this.OriginalValueLookup;
            }
        }

        object IField.OriginalValue
        {
            get
            {
                return this.OriginalValueLookup;
            }
        }

        public object OriginalDereferencedValue
        {
            get
            {
                if (this.OriginalValueLookup.HasValue)
                {
                    var Deref = Choices.Choices.Where(c => c.ID == this.OriginalValueLookup.Value).FirstOrDefault();
                    if (Deref != null)
                    {
                        return Deref.Value;
                    }
                    return null;
                }
                return null;
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
                int PossibleIntParsed;
                if (int.TryParse((string)Value, out PossibleIntParsed))
                {
                    return this.Value.HasValue && this.Value.Value == PossibleIntParsed;
                }
                else
                {
                    return (string)this.DereferencedValue == (string)Value;
                }
            }
            else if (Value is int)
            {
                return this.Value.Value == (int)Value;
            }
            return false;
        }

        public FieldLookup()
            : base()
        { }

        public FieldLookup(FieldType Type)
            : this(Type, null, null)
        {

        }

        public FieldLookup(FieldType Type, EntityBase Entity)
            : this(Type, null, Entity)
        {

        }

        public FieldLookup(FieldType Type, FieldBase Base, EntityBase Entity, Db.IDbContext ctx = null)
            : base(Type, Base, Entity, ctx)
        {
            if (Type.DataType != DataType.Lookup)
            {
                throw new ArgumentOutOfRangeException("Cannot create a lookup field from a non-lookup type.");
            }
        }

        private LookupCollection _Choices = null;
        public LookupCollection Choices
        {
            get
            {
                if (_Choices == null)
                {
                    _Choices = PersistenceService.GetLookupCollectionById(this.Type.LookupTypeID.Value);
                }
                return _Choices;
            }
        }
    }
}
