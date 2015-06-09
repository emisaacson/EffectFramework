using System;
using System.Collections.Generic;
using System.Linq;
using EffectFramework.Core.Services;
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

        private FieldTypeMetaBasic _Meta = new FieldTypeMetaBasic(false);
        public FieldTypeMetaBasic MetaLookup
        {
            get
            {
                return _Meta;
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
                    var Deref = Choices.Where(c => c.ID == this.Value.Value).FirstOrDefault();
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
                    var Deref = Choices.Where(c => c.ID == this.OriginalValueLookup.Value).FirstOrDefault();
                    if (Deref != null)
                    {
                        return Deref.Value;
                    }
                    return null;
                }
                return null;
            }
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

        public FieldLookup(FieldType Type, FieldBase Base, EntityBase Entity)
            : base(Type, Base, Entity)
        {
            if (Type.DataType != DataType.Lookup)
            {
                throw new ArgumentOutOfRangeException("Cannot create a lookup field from a non-lookup type.");
            }
        }

        private IEnumerable<LookupEntry> _Choices = null;
        public IEnumerable<LookupEntry> Choices
        {
            get
            {
                if (_Choices == null)
                {
                    _Choices = PersistenceService.GetChoicesForLookupField(this).OrderBy(l => l.Value);
                }
                return _Choices;
            }
        }
    }
}
