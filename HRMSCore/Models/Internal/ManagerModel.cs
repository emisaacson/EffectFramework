using HRMS.Modules.DBModel;
using Local.Classes.Attributes;
using Local.Models.Employee;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Local.Models.Internal
{
    [DisplayName("Manager Information")]
    [MasterTable("ManagerMasters", typeof(ManagerMaster))]
    public class IndirectManagerModel : EmployeeModel
    {
    }
}