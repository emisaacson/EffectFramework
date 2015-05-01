using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Local.Classes.Security;
using Local.Classes.Attributes;
using System.ComponentModel;
using HRMS.Modules.DBModel.Local;
using System.ComponentModel.DataAnnotations;

namespace Local.Models.Employee
{
    [DisplayName("Allowance Payments")]
    [LocalTable("EmpAllowancePayments", typeof(EmpAllowancePayments))]
    //[EmpGeneralIDProperty("EmpID")]
    public class AllowancePaymentModel : Model
    {
        [LocalProperty("EmpAllowPayId")]
        //[RequirePermission(Permission._EMP_ALLOWANCE_PAYMENT)]
        [DisplayName("Allowance Payment ID")]
        [Key]
        public int? AllowancePaymentID { get; set; }

        [LocalProperty("EmpAllowID")]
        //[RequirePermission(Permission._EMP_ALLOWANCE_PAYMENT)]
        [DisplayName("Allowance ID")]
        public int? AllowanceID { get; set; }

        [LocalProperty("Amount")]
        //[RequirePermission(Permission._EMP_ALLOWANCE_PAYMENT)]
        [DisplayName("Payment Amount")]
        [CurrencyField]
        [Required]
        [Encrypted]
        public decimal Amount { get; set; }

        [LocalProperty("PaymentType")]
        //[RequirePermission(Permission._EMP_ALLOWANCE_PAYMENT)]
        [DisplayName("Allowance Payment Type")]
        public string PaymentType { get; set; }

        [LocalProperty("CreateDate")]
        //[RequirePermission(Permission._EMP_ALLOWANCE_PAYMENT)]
        [DisplayName("Payment Date")]
        public DateTime? AllowancePaymentCreateDate { get; set; }

        [LocalProperty("Editor")]
        //[RequirePermission(Permission._EMP_ALLOWANCE_PAYMENT)]
        [DisplayName("Editor")]
        public string Editor { get; set; }

        [LocalProperty("IsDeleted")]
        //[RequirePermission(Permission._EMP_ALLOWANCE_PAYMENT)]
        [DisplayName("Is Deleted")]
        public bool IsDeleted { get; set; }

        [LocalProperty("AllowancePaymentUnit")]
        //[RequirePermission(Permission._EMP_ALLOWANCE_PAYMENT)]
        [DisplayName("Allowance Payment Unit")]
        [Required]
        [LocalReference("Currencies", typeof(Currency), "CurID", "CurName", new string[] { "IsActive", "Y" })]
        public int? AllowancePaymentUnit { get; set; }
    }
}
