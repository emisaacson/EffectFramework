using HRMS.Modules.DBModel.Local;
using HRMS.Core.Attributes;
using HRMS.Core.Helpers;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.Employee
{
    [DisplayName("Compensation")]
    [LocalTable("EmpCompensations", typeof(EmpCompensation))]
    [EmpGeneralIDProperty("EmpID")]
    public class CompensationModel : Model
    {
        [LocalProperty("EmpCompID")]
        [DisplayName("Compensation ID")]
        [Key]
        public int? CompensationID { get; set; }

        [LocalProperty("EmpID")]
        [DisplayName("Employee General ID")]
        public int? EmpGeneralID { get; set; }

        [LocalProperty]
        [DisplayName("Payroll ID")]
        public string PayRollID { get; set; }

        [LocalProperty]
        [Required]
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        [LocalProperty]
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }

        [LocalProperty]
        [DisplayName("Salary")]
        [CurrencyField]
        [Encrypted]
        public decimal? Salary { get; set; }

        [LocalProperty]
        [Required]
        [DisplayName("Salary Unit")]
        [Choices("Y|Per Year", "M|Per Month", "W|Per Week",  "H|Per Hour", "D|Per Day")]
        public char? SalaryUnit { get; set; }

        [LocalProperty]
        [DisplayName("Employee Cost")]
        [CurrencyField]
        public decimal? EmployeeCost { get; set; }

        [LocalProperty]
        [DisplayName("Employee Cost Unit")]
        [Choices("H", "U")]
        public char? EmployeeCostUnit { get; set; }

        [LocalProperty]
        [DisplayName("Salary Currency")]
        [LocalReference("Currencies", typeof(Currency), "CurID", "CurName", new string[] { "IsActive", "Y" })]
        public int? SalaryCurrency { get; set; }

        [LocalProperty]
        [DisplayName("Target Bonus")]
        [CurrencyField]
        [Encrypted]
        public decimal? TargetBonus { get; set; }

        [LocalProperty]
        [DisplayName("Target Gross Salary")]
        [CurrencyField]
        [Encrypted]
        public decimal? TargetGrossSalary { get; set; }

        [LocalProperty]
        [DisplayName("Bonus Eligible")]
        [Choices("Y|Yes", "N|No")]
        public char? BonusEligible { get; set; }

        [LocalProperty]
        [Choices("Y|Yes", "N|No")]
        public char? IsActive { get; set; }

        [LocalProperty]
        [Choices("Y|Yes", "N|No")]
        public char? IsDeleted { get; set; }

        [LocalProperty]
        [DisplayName("Pension Contribution")]
        [Choices("Y|Yes", "N|No")]
        public char? PensionContribution { get; set; }

        [LocalProperty]
        [MaxLength(64)]
        [DisplayName("Sign On Bonus")]
        [CurrencyField]
        public decimal? SignOnBonus { get; set; }

        [LocalProperty]
        [DisplayName("First Year Bonus")]
        [CurrencyField]
        public decimal? FirstYearBonus { get; set; }

        [LocalProperty]
        [DisplayName("Relocation Allowance")]
        [CurrencyField]
        public decimal? RelocationAllowance { get; set; }

        [LocalProperty]
        [MaxLength(50)]
        [DisplayName("Pay Rate")]
        [Choices("Bimonthly", "Monthly", "Weekly")]
        public string PayRate { get; set; }

        [LocalProperty("isExecComp")]
        [DisplayName("Comp Scale")]
        [Choices("1|Executive", "0|Regular")]
        public int? IsExecComp { get; set; }

        [LocalProperty]
        [DisplayName("Job Assignment")]
        [Choices]
        public int? JobAssignID { get; set; }

        public override void Validate()
        {
            base.Validate();
            if (CookieHelper.IsUserEnteredKey())
            {
                if (Salary == null)
                {
                    Errors().ModelState.Add("", "Compensation Salary is a required field");
                }
            }
        }
    }
}
