using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                this.Dirty = true;

                if (value.HasValue && value.Value == default(int))
                {
                    this.ValueLookup = null;
                }
                else
                {
                    this.ValueLookup = value;
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
                if (!this.ValueLookup.Equals((int?)value))
                {
                    this.Dirty = true;
                    if (((int?)value).HasValue && ((int?)value).Value == default(int))
                    {
                        this.ValueLookup = null;
                    }
                    else
                    {
                        this.ValueLookup = (int?)value;
                    }
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
    }
}
