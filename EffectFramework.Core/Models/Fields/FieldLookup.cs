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
                this.ValueLookup = value;
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
                if (value == null)
                {

                }
                else
                {
                    if (value != null && !typeof(int?).IsAssignableFrom(value.GetType()))
                    {
                        throw new InvalidCastException("Must assign a int key to a lookup field.");
                    }
                    if (!this.ValueLookup.Equals((int?)value))
                    {
                        this.Dirty = true;
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
            if (Type.DataType != DataType.Person)
            {
                throw new ArgumentOutOfRangeException("Cannot create a person field from a non-person type.");
            }
            this.Type = Type;
            this.Name = Type.Name;

            if (Base != null)
            {
                LoadUpValues(Base);
            }
        }
    }
}
