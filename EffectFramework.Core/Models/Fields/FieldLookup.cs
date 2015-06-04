using System;
using System.Collections.Generic;
using System.Linq;
using EffectFramework.Core.Services;

namespace EffectFramework.Core.Models.Fields
{
    public class FieldLookup : FieldBase, IField
    {
        public string Name { get; private set; }
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

        public FieldLookup(IPersistenceService PersistenceService)
            : base(PersistenceService)
        { }

        public FieldLookup(FieldType Type, IPersistenceService PersistenceService)
            : this(Type, null, PersistenceService)
        {

        }

        public FieldLookup(FieldType Type, FieldBase Base, IPersistenceService PersistenceService)
            : base(PersistenceService)
        {
            if (Type.DataType != DataType.Lookup)
            {
                throw new ArgumentOutOfRangeException("Cannot create a lookup field from a non-lookup type.");
            }
            this.Type = Type;
            this.Name = Type.Name;

            if (Base != null)
            {
                LoadUpValues(Base);
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
    }
}
