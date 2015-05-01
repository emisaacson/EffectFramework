using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HRMS.Core.Classes.Security;
using HRMS.Core.Classes.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Local.Classes.Helpers;

namespace Local.Models.Employee
{
    [DisplayName("Allowances")]
    [LocalTable("EmpAllowances", typeof(EmpAllowance))]
    [EmpGeneralIDProperty("EmpID")]
    public class AllowanceModel : Model
    {
        [LocalProperty("EmpAllowID")]
        [DisplayName("Allowance ID")]
        [Key]
        public int? AllowanceID { get; set; }

        [LocalProperty("AllowanceType")]
        [DisplayName("Allowance Type")]
        [LocalReference("EmpAllowanceTypes", typeof(EmpAllowanceType), "EmpAllowID", "TypeName", new string[] { "IsActive", "Y" })]
        [Required]
        public int? Type { get; set; }

        [LocalProperty("AllowanceAmount")]
        [DisplayName("Allowance Amount")]
        [CurrencyField]
        [Encrypted]
        public decimal? Amount { get; set; }

        [LocalProperty("AllowanceUnit")]
        [DisplayName("Allowance Currency")]
        [LocalReference("Currencies", typeof(Currency), "CurID", "CurName", new string[] { "IsActive", "Y" })]
        public int? Currency { get; set; }

        [LocalProperty]
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        [LocalProperty]
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }

        [LocalProperty("AllowancePeriod")]
        [DisplayName("Frequency")]
        [Choices("Annual", "Semi-Annual", "Quarterly", "Monthly", "Semi-Monthly", "Weekly", "One Time")]
        public string Frequency { get; set; }

        [LocalProperty]
        [Textarea]
        public string Notes { get; set; }

        [Encrypted]
        public string Total { get; set; }

        public override void Validate()
        {
            base.Validate();

            if (CookieHelper.IsUserEnteredKey())
            {
                if (Amount == null)
                {
                    Errors().ModelState.Add("", "Allowance Amount is a required field");
                }
            }
        }
    }
}
