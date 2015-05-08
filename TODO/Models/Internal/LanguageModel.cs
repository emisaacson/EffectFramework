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
    [DisplayName("Languages")]
    [LocalTable("Languages", typeof(Language))]
    public class LanguageModel : Model
    {
        [DisplayName("Language ID")]
        //[RequirePermission(Permission._LANGUAGES)]
        [LocalProperty("LangID")]
        [Key]
        public int? LanguageID { get; set; }

        [DisplayName("Language Name")]
        //[RequirePermission(Permission._LANGUAGES)]
        [LocalProperty("LangName")]
        public string LanguageName { get; set; }
    }
}