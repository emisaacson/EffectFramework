using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HRMS.Core.Models.Employee;
using HRMS.Modules.DBModel;

namespace HRMS.Core.Models.ViewModels
{
    public class EmployeeTransferViewModel
    {
        public EmployeeTransferModel TransferEmployee { get; set; }
        public JobAssignmentsCollection JobInfo { get; set; }
        public List<EntityMaster> Entities { get; set; }
        public List<usp_AllEmployeesResult> Employees { get; set; }
    }
}
