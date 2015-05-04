using HRMS.Modules.DBModel;
using HRMS.Core.Attributes;
using HRMS.Core.Models.Employee;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.Internal
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