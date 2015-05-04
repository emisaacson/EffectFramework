using HRMS.Core.Models.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.ViewModels
{
    public class Employee_AuditLogViewModel
    {
        public ModelCollection<AuditLogModel> AuditLog { get; set; }
        public EmployeeModel BasicEmployee { get; set; }
    }
}