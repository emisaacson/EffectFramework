using HRMS.Modules.DBModel;
using HRMS.Core.Attributes;
using HRMS.Core.Models.Employee;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.Internal
{
    [DisplayName("Manager Information")]
    [MasterTable("ManagerMasters", typeof(ManagerMaster))]
    public class IndirectManagerModel : EmployeeModel
    {
    }
}