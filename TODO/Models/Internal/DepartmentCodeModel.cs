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
    [DisplayName("Department Code")]
    [MasterTable("GlobalDeptCodes", typeof(GlobalDeptCode))]
    public class DepartmentCodeModel : Model
    {
        [DisplayName("Department Code ID")]
        //[RequirePermission(Permission._EMP_JOB)]
        [MasterProperty("GlobalDCID")]
        [Key]
        public int? GlobalDCID { get; set; }

        [DisplayName("Department Code Number")]
        //[RequirePermission(Permission._EMP_JOB)]
        [MasterProperty("GlobalDCCode")]
        [MaxLength(50)]
        public string DepartmentCodeNumber { get; set; }

        [DisplayName("Department Code Name")]
        //[RequirePermission(Permission._EMP_JOB)]
        [MasterProperty("GlobalDCName")]
        [MaxLength(100)]
        public string DepartmentCodeName { get; set; }
    }
}