using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HRMS.Core.Attributes;
using System.ComponentModel;
using HRMS.Modules.DBModel;
using HRMS.Core.Security;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Core.Models.Internal
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