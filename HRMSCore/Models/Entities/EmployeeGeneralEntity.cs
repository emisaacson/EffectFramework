using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Core.Models.Fields;
using HRMS.Core.Services;

namespace HRMS.Core.Models.Entities
{
    public class EmployeeGeneralEntity : EntityBase
    {
        public override EntityType Type
        {
            get
            {
                return EntityType.Employee_General;
            }
        }

        public EmployeeGeneralEntity() : base()
        {

        }



        public EmployeeGeneralEntity(IPersistenceService PersistenceService)
            : base(PersistenceService)
        {
        }

        protected override void WireUpFields()
        {
            HireDate = new FieldDate(FieldType.Hire_Date, PersistenceService);
        }

        public FieldDate HireDate { get; private set; }
    }
}
