using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Core.Models.Fields
{
    public class FieldBase
    {
        public FieldType Type { get; protected set; }

        protected string ValueString { get; set; }
        protected DateTime? ValueDate { get; set; }
        protected decimal? ValueDecimal { get; set; }
        protected bool? ValueBool { get; set; }
        protected int? ValuePerson { get; set; }

        protected void LoadUpValues(FieldBase Base)
        {
            this.ValueString  = Base.ValueString;
            this.ValueDate    = Base.ValueDate;
            this.ValueDecimal = Base.ValueDecimal;
            this.ValueBool    = Base.ValueBool;
            this.ValuePerson  = Base.ValuePerson;
        }

        public FieldBase() { }

        public FieldBase(string ValueString, DateTime? ValueDate, decimal? ValueDecimal, bool? ValueBool, int? ValuePerson)
        {
            this.ValueString  = ValueString;
            this.ValueDate    = ValueDate;
            this.ValueDecimal = ValueDecimal;
            this.ValueBool    = ValueBool;
            this.ValuePerson  = ValuePerson;
        }

        public void LoadUpValues(FieldType Type, FieldBase Base)
        {
            this.Type = Type;
            LoadUpValues(Base);
        }
    }
}
