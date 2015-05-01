using HRMS.Modules.DBModel.Local;
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
    [DisplayName("Allowance Types")]
    [LocalTable("EmpAllowanceTypes", typeof(EmpAllowanceType))]
    public class AllowanceTypeModel : Model
    {
        [DisplayName("Allowance Type ID")]
        //[RequirePermission(Permission._EMP_ALLOW)]
        [LocalProperty("EmpAllowID")]
        [Key]
        public int? AllowanceTypeID { get; set; }

        [DisplayName("Allowance Type")]
        //[RequirePermission(Permission._EMP_ALLOW)]
        [LocalProperty("TypeName")]
        public string AllowanceTypeName { get; set; }
    }
}