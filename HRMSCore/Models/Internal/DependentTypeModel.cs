using HRMS.Modules.DBModel.Local;
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
    [DisplayName("Dependent Types")]
    [LocalTable("EmpDependentTypes", typeof(EmpDependentType))]
    public class DependentTypeModel : Model
    {
        [DisplayName("Dependent Type ID")]
        //[RequirePermission(Permission._DEPENDENTS)]
        [LocalProperty("DepTypeID")]
        [Key]
        public int? DependentTypeID { get; set; }

        [DisplayName("Dependent Type")]
        //[RequirePermission(Permission._DEPENDENTS)]
        [LocalProperty("DepTypeName")]
        public string DependentTypeName { get; set; }
    }
}