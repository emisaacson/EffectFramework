using System;
using HRMS.Core.Models.Entities;
using HRMS.Core.Services;

namespace HRMS.Core.Models.Fields
{
    public class FieldBase
    {
        public FieldType Type { get; protected set; }
        public Guid Guid { get; protected set; }
        public int? FieldID { get; protected set; }

        protected string ValueString { get; set; }
        protected DateTime? ValueDate { get; set; }
        protected decimal? ValueDecimal { get; set; }
        protected bool? ValueBool { get; set; }
        protected int? ValuePerson { get; set; }
        protected readonly IPersistenceService PersistenceService;

        protected void LoadUpValues(FieldBase Base)
        {
            if (Base == null)
            {
                this.FieldID = null;
                this.ValueString = null;
                this.ValueDate = null;
                this.ValueDecimal = null;
                this.ValueBool = null;
                this.ValuePerson = null;
            }
            else
            {
                this.FieldID = Base.FieldID;
                this.ValueString = Base.ValueString;
                this.ValueDate = Base.ValueDate;
                this.ValueDecimal = Base.ValueDecimal;
                this.ValueBool = Base.ValueBool;
                this.ValuePerson = Base.ValuePerson;
            }
        }

        public FieldBase(IPersistenceService PersistenceService)
        {
            this.PersistenceService = PersistenceService;
        }

        public FieldBase(int? FieldID, string ValueString, DateTime? ValueDate, decimal? ValueDecimal, bool? ValueBool, int? ValuePerson)
        {
            this.FieldID = FieldID;
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
