using HRMS.Modules.DBModel;
using HRMS.Core.Attributes;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Core.Unused
{
    [DisplayName("Entities")]
    [MasterTable("EntityMasters", typeof(EntityMaster))]
    public class EntityMasterModel : Model
    {
        [DisplayName("Entity ID")]
        //[RequirePermission(Permission._EMP_BASIC)]
        [MasterProperty("EntityMasterID")]
        [Key]
        public int? EntityID { get; set; }

        [DisplayName("Entity Name")]
        //[RequirePermission(Permission._EMP_BASIC)]
        [MasterProperty]
        public string EntityName { get; set; }

        [DisplayName("Database Name")]
        //[RequirePermission(Permission._EMP_BASIC)]
        [MasterProperty("DBName")]
        public string DatabaseName { get; set; }
    }
}