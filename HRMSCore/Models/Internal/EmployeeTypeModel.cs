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