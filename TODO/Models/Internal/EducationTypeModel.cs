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
    [DisplayName("Education Degrees")]
    [LocalTable("EmpEducationTypes", typeof(EmpEducationType))]
    public class EducationTypeModel : Model
    {
        [DisplayName("Education Type ID")]
        //[RequirePermission(Permission._EMP_BASIC)]
        [LocalProperty("EduTypeID")]
        [Key]
        public int? EducationTypeID { get; set; }

        [DisplayName("Degree")]
        //[RequirePermission(Permission._EMP_BASIC)]
        [LocalProperty("EduTypeName")]
        public string EducationTypeName { get; set; }
    }
}