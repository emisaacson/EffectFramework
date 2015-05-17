using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EffectFramework.Core.Services;

namespace EffectFramework.Core.Models.Fields
{
    public class FieldPerson : FieldBase, IField
    {
        public string Name { get; private set; }
        public int? Value
        {
            get
            {
                return this.ValuePerson;
            }
            set
            {
                this.Dirty = true;
                this.ValuePerson = value;
            }
        }

        object IField.Value
        {
            get
            {
                return this.ValuePerson;
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
                        throw new InvalidCastException("Must assign a int key to a person field.");
                    }
                    if (!this.ValuePerson.Equals((int?)value))
                    {
                        this.Dirty = true;
                        this.ValuePerson = (int?)value;
                    }
                }
            }
        }

        public FieldPerson(IPersistenceService PersistenceService)
            : base(PersistenceService)
        { }

        public FieldPerson(FieldType Type, IPersistenceService PersistenceService)
            : this(Type, null, PersistenceService)
        {

        }

        public FieldPerson(FieldType Type, FieldBase Base, IPersistenceService PersistenceService)
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
