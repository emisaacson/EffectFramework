using HRMS.Core.Attributes;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HRMS.Modules.DBModel.Local;

namespace HRMS.Core.Models.Employee
{
    [DisplayName("Education")]
    [LocalTable("EmpEducations", typeof(EmpEducation))]
    [EmpGeneralIDProperty("EmpID")]
    public class EducationModel : Model
    {
        [LocalProperty("EmpEduID")]
        //[RequirePermission(Permission._EDUCATION)]
        [DisplayName("Education ID")]
        [Key]
        //[SelfService]
        public int? EducationID { get; set; }

        [LocalProperty("EmpEduType")]
        //[RequirePermission(Permission._EDUCATION)]
        [DisplayName("Education Type")]
        [Choices("1|Formal")]
        [Required]
        [HideInCustomReports]
        //[SelfService]
        public int? Type { get; set; }

        [LocalProperty("DegreeOrCert")]
        //[RequirePermission(Permission._EDUCATION)]
        [LocalReference("EmpEducationTypes", typeof(EmpEducationType), "EduTypeID", "EduTypeName", new string[] { "IsActive", "Y", "ParentID", "1" })]
        [Required]
        //[SelfService]
        [HideInCustomReports]
        public int? Degree { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EDUCATION)]
        [Required]
        //[SelfService]
        public string Institution { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EDUCATION)]
        [DisplayName("Area of Study")]
        //[SelfService]
        public string AreaOfStudy { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EDUCATION)]
        [DisplayName("Date Completed")]
        //[SelfService]
        public DateTime? DateCompleted { get; set; }
    }
}
