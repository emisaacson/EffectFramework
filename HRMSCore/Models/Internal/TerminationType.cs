using HRMS.Modules.DBModel;
using Local.Classes.Attributes;
using Local.Models.Employee;
using Local.Classes.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Local.Models.Internal
{
    [DisplayName("Termination Reason")]
    [MasterTable("TerminationReasons", typeof(TerminationReason))]
    public class TerminationType : Model
    {
        //[RequirePermission(Permission._TERMINATION_INFO)]
        [MasterProperty]
        [DisplayName("Termination Reason")]
        public string TermReasonName { get; set; }

        //[RequirePermission(Permission._TERMINATION_INFO)]
        [MasterProperty]
        [DisplayName("Termination Type")]
        public string Category { get; set; }
    }
}