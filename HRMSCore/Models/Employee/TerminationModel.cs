using HRMS.Modules.DBModel;
using Local.Classes.Attributes;
using Local.Classes.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Local.Models.Employee
{
    [DisplayName("Termination")]
    [MasterTable("EmpTerminationMasters", typeof(EmpTerminationMaster))]
    [EmpGeneralIDProperty("EmpGeneralID")]
    public class TerminationModel : Model
    {
        [Required]
        [MasterProperty]
        [DisplayName("Termination ID")]
        //[RequirePermission(Permission._TERMINATION_INFO)]
        [Key]
        public int EmpTermID { get; set; }

        [Required]
        [MasterProperty]
        [DisplayName("Termination Date")]
        //[RequirePermission(Permission._TERMINATION_INFO)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? EmpTermDate { get; set; }

        [Required]
        [MasterProperty("EmpTermReason")]
        [MasterReference("TerminationReasons", typeof(TerminationReason), "TermReasonID", "TermReasonDec", new string[] { "IsActive", "Y" })]
        [DisplayName("Termination Type / Reason")]
        //[RequirePermission(Permission._TERMINATION_INFO)]
        public int? TerminationReason { get; set; }

        [MasterProperty]
        [DisplayName("Pay Through Date")]
        //[RequirePermission(Permission._TERMINATION_INFO)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? PayThruDate { get; set; }

        [MasterProperty]
        [DisplayName("Disable Account Date")]
        //[RequirePermission(Permission._TERMINATION_INFO)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DisableAccountDate { get; set; }

        [MasterProperty]
        [Choices("Yes", "No")]
        [DisplayName("Loss to the company")]
        //[RequirePermission(Permission._TERMINATION_INFO)]
        public string IsLoss { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._TERMINATION_INFO)]
        [Textarea]
        public string Notes { get; set; }
    }
}
