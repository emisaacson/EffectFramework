using HRMS.Modules.DBModel;
using HRMS.Core.Attributes;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Core.Unused
{
    [DisplayName("Cost Codes")]
    [MasterTable("FinanceCostCodes", typeof(FinanceCostCode))]
    public class CostCodeModel : Model
    {
        [MasterProperty("FinCCID")]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Cost Code ID")]
        [Key]
        public int? CostCodeID { get; set; }

        [MasterProperty("FinCCode")]
        //[RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Cost Code Number")]
        [MaxLength(50)]
        public string CostCodeNumber { get; set; }

        [MasterProperty("FinCCName")]
        //[RequirePermission(Permission._EMP_JOB)]
        [MaxLength(50)]
        [DisplayName("Cost Code Name")]
        public string CostCodeName { get; set; }

		[MasterProperty("FinCCDesc")]
		[DisplayName("Cost Code Description")]
		public string CostCodeDescription { get; set; }

        [MasterProperty]
        //[RequirePermission(Permission._EMP_JOB)]
        [MasterReference("ExpenseGroups", typeof(ExpenseGroup), "ExpenseGroupID", "ExpenseGroupName", new string[] { "IsDeleted", "False" })]
        [DisplayName("Expense Group ID")]
        public int? ExpenseGroupID { get; set; }
    }
}