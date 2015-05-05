using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace HRMS.Core.Models.Fields
{
    public class FieldDate : FieldBase, IField
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

        public FieldDate() { }

        public FieldDate(FieldType Type)
            : this(Type, null)
        {

        }

        public FieldDate(FieldType Type, FieldBase Base)
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