using HRMS.Modules.DBModel.Local;
using HRMS.Core.Attributes;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.Models
{

    public class AccessControllListModel
    {
        [LocalProperty("ACLAssignID")]
        //[RequirePermission(Permission._SYSTEM_SETTINGS)]
        [DisplayName("ACL Id")]
        [Key]
        [Required]
        public int? AccessControllId { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._SYSTEM_SETTINGS)]
        [Required]
        public char? AccessLevel { get; set; }
        
        public string SecurityObjectName { get; set; }

        public int SecurityId { get; set; }
    }
}