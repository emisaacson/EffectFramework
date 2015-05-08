using HRMS.Modules.DBModel.Local;
using HRMS.Core.Attributes;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.Employee
{
    public class EducationCollection : ModelCollection<EducationModel>
    {
        //[RequirePermission(Permission._EDUCATION)]
        public override IEnumerable<EducationModel> Items { get; set; }

        public IEnumerable<EmpEducationType> EducationTypes { get; set; }
    }
}