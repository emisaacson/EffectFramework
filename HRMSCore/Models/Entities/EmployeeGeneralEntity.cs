﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Core.Models.Fields;

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

        public readonly FieldDate HireDate = new FieldDate(FieldType.Hire_Date);
    }
}
