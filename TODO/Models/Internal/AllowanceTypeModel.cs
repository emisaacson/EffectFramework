using HRMS.Modules.DBModel.Local;
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