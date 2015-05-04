using HRMS.Modules.DBModel;
using HRMS.Core.Models.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.ViewModels
{
    public class TerminateEmployeeViewModel
    {
        public TerminateEmployeeModel TerminateEmployee { get; set; }
        public List<EntityMaster> Entities { get; set; }
        public List<usp_AllEmployeesResult> Employees { get; set; }
        public bool? DontSendToSP { get; set; }
    }
}
