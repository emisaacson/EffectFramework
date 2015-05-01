using HRMS.Modules.DBModel.Local;
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
    [DisplayName("Security Questions")]
    [LocalTable("EmpSecurities", typeof(EmpSecurity))]
    [EmpGeneralIDProperty("EmpGeneralID")]
    public class SecurityQuestionModel : Model
    {
        [LocalProperty("EmpSecurityId")]
        ////[RequirePermission(Permission._SECURITY_QUESTIONS)]
        [DisplayName("Question ID")]
        [Key]
        //[SelfService]
        public int? EmpSecurityId { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._SECURITY_QUESTIONS)]
        [DisplayName("Question")]
        [Required]
        //[SelfService]
        public string Question { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._SECURITY_QUESTIONS)]
        [DisplayName("Answer")]
        [Required]
        //[SelfService]
        public string Answer { get; set; }

    }
}
