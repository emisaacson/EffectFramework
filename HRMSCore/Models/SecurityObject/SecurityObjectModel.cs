using System;
using System.Collections.Generic;
using Local.Classes.Attributes;
using Local.Classes.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Local.Models.SecurityObject
{
    public class SecurityObjectModel : Model
    {
        [LocalProperty("SecObjID")]
        [RequirePermission(Permission._SYSTEM_SETTINGS)]
        [DisplayName("Security Object ID")]
        [Key]
        public int SecObjID { get; set; }

        [LocalProperty("FriendlyName")]
        [RequirePermission(Permission._SYSTEM_SETTINGS)]
        [DisplayName("Friendly Name")]
        public string FriendlyName { get; set; }

        [Required]
        [LocalProperty("SecObjName")]
        [RequirePermission(Permission._SYSTEM_SETTINGS)]
        [DisplayName("Name")]
        public string SecObjName { get; set; }

        [LocalProperty("SecObjDesc")]
        [RequirePermission(Permission._SYSTEM_SETTINGS)]
        [DisplayName("Description")]
        public string SecObjDesc { get; set; }

        [LocalProperty("SecObjAttribute")]
        [RequirePermission(Permission._SYSTEM_SETTINGS)]
        [DisplayName("Attribute")]
        public string SecObjAttribute { get; set; }

        [LocalProperty]
        [RequirePermission(Permission._SYSTEM_SETTINGS)]
        public char? IsActive { get; set; }
    }
}