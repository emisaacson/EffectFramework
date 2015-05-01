using HRMS.Modules.DBModel;
using Local.Classes.Attributes;
using Local.Classes.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Local.Models.Internal
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