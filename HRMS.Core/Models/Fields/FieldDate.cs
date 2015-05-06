using System;
using HRMS.Core.Services;

namespace HRMS.Core.Models.Fields
{
    public class FieldDate : FieldBase, IField
    {
        public string Name { get; private set; }
        public DateTime Value
        {
            get
            {
                return this.ValueDate.Value;
            }
        }

        object IField.Value
        {
            get
            {
                return this.ValueDate.Value;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public FieldDate(IPersistenceService PersistenceService)
            : base(PersistenceService)
        { }

        public FieldDate(FieldType Type, IPersistenceService PersistenceService)
            : this(Type, null, PersistenceService)
        {

        }

        public FieldDate(FieldType Type, FieldBase Base, IPersistenceService PersistenceService)
            : base(PersistenceService)
        {
            if (Type.DataType != DataType.Date)
            {
                throw new ArgumentOutOfRangeException("Cannot create a date field from a non-date type.");
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