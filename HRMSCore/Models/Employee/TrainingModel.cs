using Local.Classes.Attributes;
using Local.Classes.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HRMS.Modules.DBModel.Local;

namespace Local.Models.Employee
{
    [DisplayName("Training")]
    [LocalTable("EmpEducation", typeof(EmpEducation))]
    [EmpGeneralIDProperty("EmpID")]
    public class TrainingModel : Model
    {
        [LocalProperty("EmpEduID")]
        //[RequirePermission(Permission._EDUCATION)]
        //[SelfService]
        [DisplayName("Education ID")]
        [Key]
        public int? EducationID { get; set; }

        [LocalProperty("EmpEduType")]
        //[RequirePermission(Permission._EDUCATION)]
        [DisplayName("Training Type")]
        [Choices("2|Training", "3|Certification", "10|Apprenticeship", "11|Professional Association")]
        [HideInCustomReports]
        //[SelfService]
        [Required]
        public int? Type { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EDUCATION)]
        //[SelfService]
        [Required]
        public string Institution { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EDUCATION)]
        //[SelfService]
        [DisplayName("Area of Study")]
        public string AreaOfStudy { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EDUCATION)]
        //[SelfService]
        [DisplayName("Date Completed")]
        public DateTime? DateCompleted { get; set; }
    }
}