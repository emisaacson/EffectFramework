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
    [DisplayName("Indirect Manager Information")]
    [MasterTable("IndirectManagerMasters", typeof(IndirectManagerMaster))]
    public class ManagerModel : EmployeeModel
    {
    }
}