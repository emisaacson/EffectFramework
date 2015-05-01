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
    [DisplayName("Email Template")]
    [MasterTable("LetterTemplate", typeof(LetterTemplate))]
    public class EmailTemplateModel : Model
    {
        [MasterProperty("LetterTemplateId")]
       // [RequirePermission(Permission._EMP_JOB)]
        [DisplayName("Email Template ID")]
        [Key]
        public int LetterTemplateId { get; set; }

        
        [DisplayName("Template name")]
        //[RequirePermission(Permission._SYSTEM_SETTINGS)]
        [MasterProperty("Name")]
        public string Name { get; set; }

        [MasterProperty]
        public DateTime CreateDateUtc { get; set; }

        [MasterProperty]
        public bool IsDelete { get; set; }

        [MasterProperty]
        public string ExtraFieldsJson { get; set; }

        [MasterProperty]
        public string TemplateHtml { get; set; }

        [MasterProperty]
        public string IsEmailTemplate { get; set; }

    }
}