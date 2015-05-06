using System;
using HRMS.Core.Services;

namespace HRMS.Core.Models.Fields
{
    public class FieldString : FieldBase, IField
    {
        public int? FieldID { get; private set; }
        public string Name { get; private set; }
        public string Value
        {
            get
            {
                return this.ValueString;
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
                throw new NotImplementedException();
            }
        }

        public FieldString(IPersistenceService PersistenceService)
            : base(PersistenceService)
        { }

        public FieldString(FieldType Type, IPersistenceService PersistenceService)
            : this(Type, null, PersistenceService)
        {

        }

        public FieldString(FieldType Type, FieldBase Base, IPersistenceService PersistenceService)
            : base(PersistenceService)
        {
            if (Type.DataType != DataType.Text)
            {
                throw new ArgumentOutOfRangeException("Cannot create a string field from a non-string type.");
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
