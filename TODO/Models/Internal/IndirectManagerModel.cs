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
    [DisplayName("Indirect Manager Information")]
    [MasterTable("IndirectManagerMasters", typeof(IndirectManagerMaster))]
    public class ManagerModel : EmployeeModel
    {
    }
}