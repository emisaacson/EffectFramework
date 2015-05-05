using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Core.Models.Fields;

namespace HRMS.Core.Models.Entities
{
    public class EmployeeGeneralEntity : IEntity
    {
        public int? EntityID { get; private set; }
        public int? EmployeeID { get; private set; }
        public EntityType Type
        {
            get
            {
                return EntityType.Employee_General;
            }
        }

        public readonly FieldDate HireDate = new FieldDate(FieldType.Hire_Date);
    }
}
