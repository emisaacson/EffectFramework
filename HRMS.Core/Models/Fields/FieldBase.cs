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
        public bool Dirty { get; protected set; }

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
                this.Dirty        = true;
                this.FieldID      = null;
                this.ValueString  = null;
                this.ValueDate    = null;
                this.ValueDecimal = null;
                this.ValueBool    = null;
                this.ValuePerson  = null;
            }
            else
            {
                this.Dirty        = false;
                this.FieldID      = Base.FieldID;
                this.ValueString  = Base.ValueString;
                this.ValueDate    = Base.ValueDate;
                this.ValueDecimal = Base.ValueDecimal;
                this.ValueBool    = Base.ValueBool;
                this.ValuePerson  = Base.ValuePerson;
                this.Guid         = Base.Guid;
            }
        }

        public FieldBase(IPersistenceService PersistenceService)
        {
            this.Dirty = true;
            this.PersistenceService = PersistenceService;
        }

        public FieldBase(Db.EntityField Field)
        {
            this.Dirty        = true;
            this.FieldID      = Field.EntityFieldID;
            this.ValueString  = Field.ValueText;
            this.ValueDate    = Field.ValueDate;
            this.ValueDecimal = Field.ValueDecimal;
            this.ValueBool    = Field.ValueBoolean;
            this.ValuePerson  = Field.ValueUser;
            this.Guid         = Field.Guid;
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
