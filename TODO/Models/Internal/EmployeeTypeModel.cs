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
    [DisplayName("Employee Type")]
    [LocalTable("EmployeeTypes", typeof(EmployeeType))]
    public class EmployeeTypeModel : Model
    {
        [DisplayName("Employee Type ID")]
        //[RequirePermission(Permission._EMP_BASIC)]
        [LocalProperty("EmpTypeID")]
        [Key]
        public int? EmployeeTypeID { get; set; }

        [DisplayName("Employee Type")]
        //[RequirePermission(Permission._EMP_BASIC)]
        [LocalProperty("TypeName")]
        public string EmployeeTypeName { get; set; }
    }
}