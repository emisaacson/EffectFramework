using HRMS.Modules.DBModel;
using HRMS.Core.Attributes;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.Internal
{
    [DisplayName("Expense Group")]
    [MasterTable("ExpenseGroups", typeof(ExpenseGroup))]
    public class ExpenseGroupModel : Model
    {
        [DisplayName("Expense Group ID")]
        //[RequirePermission(Permission._EMP_JOB)]
        [MasterProperty]
        [Key]
        public int? ExpenseGroupID { get; set; }

        [DisplayName("Expense Group")]
        //[RequirePermission(Permission._EMP_JOB)]
        [MasterProperty]
        public string ExpenseGroupName { get; set; }
    }
}