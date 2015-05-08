using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HRMS.Core.Models.Employee;
using HRMS.Modules.DBModel;

namespace HRMS.Core.Models.ViewModels
{
    public class NewEmployeeViewModel
    {
        public JobAssignmentsCollection JobInfo { get; set; }
        public EntityMaster Entity { get; set; }
        public NewEmployeeModel NewEmployee { get; set; }
        public List<EntityMaster> Entities { get; set; }
        public bool IsLogicUser { get; set; }
        public bool IsGuestAccountForm { get; set; }
        public bool IsSecondaryEmail { get; set; }
        public bool? DontSendToSP { get; set; }
    }
}
