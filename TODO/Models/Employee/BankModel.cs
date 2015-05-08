using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HRMS.Core.Attributes;
using HRMS.Core.Security;
using HRMS.Modules.DBModel.Local;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Core.Models.Employee
{
    [DisplayName("Bank Information")]
    [LocalTable("EmpBankInfos", typeof(EmpBankInfo))]
    [EmpGeneralIDProperty("EmpID")]
    public class BankModel : Model
    {
        [LocalProperty("EmpBankInfoID")]
        //[RequirePermission(Permission._BANK_INFO)]
        [DisplayName("Bank ID")]
        [Key]
        public int? BankID { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._BANK_INFO)]
        [DisplayName("Account Type")]
        [Choices("Payroll Deposit", "Expense Account")]
        [MaxLength(50)]
        [Required]
        public string AccountType { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._BANK_INFO)]
        [Choices("Y|Yes", "N|No")]
        public char? IsActive { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._BANK_INFO)]
        [DisplayName("Notes")]
        [Textarea]
        public string Notes { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._BANK_INFO)]
        [DisplayName("Bank Name")]
        [MaxLength(128)]
        public string BankName { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._BANK_INFO)]
        [DisplayName("Branch Location")]
        [MaxLength(128)]
        public string BranchLocation { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._BANK_INFO)]
        [DisplayName("Branch Phone")]
        [MaxLength(64)]
        public string BranchPhone { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._BANK_INFO)]
        [DisplayName("Routing Number")]
        [Encrypted]
        [MaxLength(128)]
        public string RoutingNumber { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._BANK_INFO)]
        [DisplayName("Swift Number")]
        [Encrypted]
        [MaxLength(128)]
        public string SwiftNumber { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._BANK_INFO)]
        [Encrypted]
        [MaxLength(128)]
        public string IBAN { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._BANK_INFO)]
        [Encrypted]
        [DisplayName("Account Number")]
        [MaxLength(128)]
        public string AccountNumber { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._BANK_INFO)]
        [Encrypted]
        [MaxLength(64)]
        public string Beneficiary { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._BANK_INFO)]
        [DisplayName("Sort Code")]
        [MaxLength(64)]
        public string SortCode { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._BANK_INFO)]
        [DisplayName("Bank Address")]
        [MaxLength(1024)]
        public string BankAddress { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._BANK_INFO)]
        [MaxLength(1024)]
        public string BIC { get; set; }

        [LocalProperty]
        public bool Encrypted { get { return true; } set { } }

    }
}