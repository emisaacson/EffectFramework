using HRMS.Modules.DBModel.Local;
using HRMS.Core.Attributes;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.Employee
{
    [DisplayName("Employee Languages")]
    [LocalTable("EmpLanguages", typeof(EmpLanguage))]
    [EmpGeneralIDProperty("EmpGenID")]
    public class LanguageModel : Model
    {
        [LocalProperty("EmpLangID")]
        //[RequirePermission(Permission._LANGUAGES)]
        [DisplayName("Language ID")]
        [Key]
        //[SelfService]
        public int? LanguageID { get; set; }

        [LocalProperty("LangID")]
        //[RequirePermission(Permission._LANGUAGES)]
        [DisplayName("Language")]
        [LocalReference("Languages", typeof(Language), "LangID", "LangName", new string[] { "IsActive", "Y" })]
        [Required]
        //[SelfService]
        public int? Language { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._LANGUAGES)]
        [DisplayName("Reading")]
        [MaxLength(50)]
        [Required]
        [Choices("None", "Fair", "Good", "Fluent", "Native")]
        //[SelfService]
        public string CanRead { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._LANGUAGES)]
        [DisplayName("Writing")]
        [MaxLength(50)]
        [Required]
        [Choices("None", "Fair", "Good", "Fluent", "Native")]
        //[SelfService]
        public string CanWrite { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._LANGUAGES)]
        [DisplayName("Understanding")]
        [MaxLength(50)]
        [Required]
        [Choices("None", "Fair", "Good", "Fluent", "Native")]
        //[SelfService]
        public string CanUnderstand { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._LANGUAGES)]
        [DisplayName("Speaking")]
        [MaxLength(50)]
        [Required]
        [Choices("None", "Fair", "Good", "Fluent", "Native")]
        //[SelfService]
        public string CanSpeak { get; set; }
    }
}
