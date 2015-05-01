using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Local.Classes.Attributes;
using System.ComponentModel;
using HRMS.Modules.DBModel;
using Local.Classes.Security;
using System.ComponentModel.DataAnnotations;

namespace Local.Models.Internal
{
    [DisplayName("Divisions")]
    [MasterTable("DivisionMasters", typeof(DivisionMaster))]
    public class DivisionModel : Model
    {
        [DisplayName("Division ID")]
        [MasterProperty("DivMasterID")]
        //[RequirePermission(Permission._EMP_JOB)]
        [Key]
        public int? DivisionID { get; set; }

        [DisplayName("Division Name")]
        [MasterProperty("DivName")]
        //[RequirePermission(Permission._EMP_JOB)]
        public string DivisionName { get; set; }
    }
}