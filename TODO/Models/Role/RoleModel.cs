using HRMS.Core.Attributes;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.Role
{
    public class RoleModel : Model
    {
        [LocalProperty("RoleId")]
        //[RequirePermission(Permission._SYSTEM_SETTINGS)]
        [DisplayName("Role Id")]
        [Key]
        public int LocalRoleId { get; set; }

        [Required]
        [LocalProperty("Name")]
        //[RequirePermission(Permission._SYSTEM_SETTINGS)]
        [DisplayName("Role Name")]
        public string Name { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._SYSTEM_SETTINGS)]
        public bool IsDeleted { get; set; }


    }
}
