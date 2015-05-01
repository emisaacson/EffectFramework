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
    [DisplayName("Companies")]
    [MasterTable("CompanyMasters", typeof(CompanyMaster))]
    public class CompanyModel : Model
    {
        [DisplayName("Company ID")]
        //[RequirePermission(Permission._EMP_BASIC)]
        [MasterProperty("CompMasterID")]
        [Key]
        public int? CompanyID { get; set; }

        [DisplayName("Company Name")]
        //[RequirePermission(Permission._EMP_BASIC)]
        [MasterProperty]
        public string CompanyName { get; set; }
    }
}