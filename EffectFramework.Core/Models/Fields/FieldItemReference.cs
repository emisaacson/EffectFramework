using EffectFramework.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models.Fields
{
    [Serializable]
    public class FieldItemReference : FieldBase, IField
    {
        public int? Value
        {
            get
            {
                return this.ValueItemReference;
            }
            set
            {
                int? _Value = value;
                if (_Value.HasValue && _Value.Value == default(int))
                {
                    _Value = null;
                }
                if (!this.ValueItemReference.Equals(_Value))
                {
                    this.Dirty = true;
                    this.ValueItemReference = _Value;
                }
            }
        }

        object IField.Value
        {
            get
            {
                return this.ValueItemReference;
            }

            set
            {
                if (value != null && !typeof(int?).IsAssignableFrom(value.GetType()))
                {
                    throw new InvalidCastException("Must assign a int key to an entity reference field.");
                }
                int? _Value = (int?)value;
                if (_Value.HasValue && _Value.Value == default(int))
                {
                    _Value = null;
                }
                if (!this.ValueItemReference.Equals(_Value))
                {
                    this.Dirty = true;
                    this.ValueItemReference = _Value;
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
                    Log.Error("Must assign a FieldTypeMetaBasic to an entity reference field. Value type: {0}, Field ID: {1}", value.GetType().Name, FieldID);
                    throw new InvalidCastException("Must assign a FieldTypeMetaBasic to an entity reference field.");
                }
                _Meta = (FieldTypeMetaBasic)value;
            }
        }

        [NonSerialized]
        private Item _DereferencedValue;
        public object DereferencedValue
        {
            get
            {
                if (_DereferencedValue == null && this.Value.HasValue)
                {
                    var Deref = Item.GetItemByID(this.Value.Value);
                    if (Deref.TenantID != this.TenantID)
                    {
                        throw new Exceptions.FatalException("Data error.");
                    }
                    _DereferencedValue = Deref;
                }
                return _DereferencedValue;
            }
        }

        public int? OriginalValue
        {
            get
            {
                return this.OriginalValueItemReference;
            }
        }

        object IField.OriginalValue
        {
            get
            {
                return this.OriginalValueItemReference;
            }
        }

        [NonSerialized]
        private Item _OriginalDereferencedValue;
        public object OriginalDereferencedValue
        {
            get
            {
                if (_OriginalDereferencedValue == null && this.OriginalValue.HasValue)
                {
                    var Deref = Item.GetItemByID(this.OriginalValue.Value);
                    if (Deref.TenantID != this.TenantID)
                    {
                        throw new Exceptions.FatalException("Data error.");
                    }
                    _OriginalDereferencedValue = Deref;
                }
                return _OriginalDereferencedValue;
            }
        }

        public bool ValueEquals(object Value)
        {
            if (Value == null)
            {
                return this.Value == null;
            }
            else if (Value is Item)
            {
                return ((Item)Value).ItemID.HasValue && this.Value.Value == ((Item)Value).ItemID.Value;
            }
            else if (Value is int)
            {
                return this.Value.Value == (int)Value;
            }
            return false;
        }

        public FieldItemReference()
            : base()
        { }

        public FieldItemReference(FieldType Type)
            : this(Type, null, null)
        {

        }

        public FieldItemReference(FieldType Type, EntityBase Entity)
            : this(Type, null, Entity)
        {

        }

        public FieldItemReference(FieldType Type, FieldBase Base, EntityBase Entity)
            : base(Type, Base, Entity)
        {
            if (Type.DataType != DataType.ItemReference)
            {
                throw new ArgumentOutOfRangeException("Cannot create an item reference field from a non-item-reference type.");
            }
        }
    }
}
