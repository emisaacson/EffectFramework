using HRMS.Modules.DBModel.Local;
using HRMS.Core.Attributes;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.Employee
{
    [DisplayName("Bonus")]
    [LocalTable("EmpCompBonus", typeof(EmpCompBonus))]
    [EmpGeneralIDProperty("EmpID")]
    public class BonusModel : Model
    {
        [LocalProperty("BonusID")]
        //[RequirePermission(Permission._EMP_COMP_BONUS)]
        [DisplayName("Compensation Bonus ID")]
        [Key]
        public int? BonusID { get; set; }


        [LocalProperty("EmpID")]
        //[RequirePermission(Permission._EMP_COMP)]
        [DisplayName("Employee General ID")]
        public int? EmpGeneralID { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_COMP_BONUS)]
        [Required]
        [DisplayName("Bonus Type")]
        [Choices("1|Annual Performance", "2|Spot", "3|First Year Retention", "4|Sign On")]
        public int? BonusType { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_COMP_BONUS)]
        [DisplayName("Target Date")]
        public DateTime? TargetDate { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_COMP_BONUS)]
        [DisplayName("Paid Date")]
        public DateTime? PaidDate { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_COMP_BONUS)]
        [DisplayName("Target Bonus Amount")]
        [CurrencyField]
        [Encrypted]
        public decimal? TargetBonusAmt { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_COMP_BONUS)]
        [DisplayName("Paid Bonus Amount")]
        [CurrencyField]
        [Encrypted]
        public decimal? PaidBonusAmt { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_COMP_BONUS)]
        [DisplayName("Target Bonus Currency")]
        [LocalReference("Currencies", typeof(Currency), "CurID", "CurName", new string[] { "IsActive", "Y" })]
        public int? TargetBonusCur { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_COMP_BONUS)]
        [DisplayName("Paid Bonus Currency")]
        [LocalReference("Currencies", typeof(Currency), "CurID", "CurName", new string[] { "IsActive", "Y" })]
        public int? PaidBonusCur { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_COMP_BONUS)]
        [DisplayName("Notes")]
        [Textarea]
        public string Notes { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_COMP_BONUS)]
        [Choices("Y|Yes", "N|No")]
        public char? IsActive { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_COMP_BONUS)]
        [DisplayName("Comp Scale")]
        [Choices("1|Executive", "0|Regular")]
        public int? isExecComp { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_COMP_BONUS)]
        [Choices("1|Yes","0|No" )]
        [DisplayName("Contractual Bonus")]
        [Required]
        public int? IsContractualBonus { get; set; }

        [LocalProperty("EmpCompID")]
        //[RequirePermission(Permission._EMP_COMP)]
        [DisplayName("Compensation")]
        [Choices]
        [Encrypted]
        public int? CompensationID { get; set; }


    }
}
