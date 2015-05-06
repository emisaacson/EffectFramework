using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Core.Models.Entities;
using HRMS.Core.Services;
using Ninject;

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
        protected readonly IPersistenceService PersistenceService;

        protected void LoadUpValues(FieldBase Base)
        {
            this.ValueString  = Base.ValueString;
            this.ValueDate    = Base.ValueDate;
            this.ValueDecimal = Base.ValueDecimal;
            this.ValueBool    = Base.ValueBool;
            this.ValuePerson  = Base.ValuePerson;
        }

        public FieldBase(IPersistenceService PersistenceService)
        {
            this.PersistenceService = PersistenceService;
        }

        public FieldBase(string ValueString, DateTime? ValueDate, decimal? ValueDecimal, bool? ValueBool, int? ValuePerson)
        {
            this.ValueString  = ValueString;
            this.ValueDate    = ValueDate;
            this.ValueDecimal = ValueDecimal;
            this.ValueBool    = ValueBool;
            this.ValuePerson  = ValuePerson;
        }

        public FieldBase(FieldType Type, FieldBase Base, IPersistenceService PersistenceService)
        {
            this.PersistenceService = PersistenceService;
            this.Type = Type;
            LoadUpValues(Base);
        }

        public void FillFromDatabase(EntityBase Entity, FieldBase Field)
        {
            FieldBase Base = PersistenceService.RetreiveSingleFieldOrDefault(Entity, Field.Type);
            LoadUpValues(Base);
        }
    }
}
