using Local.Classes.Attributes;
using Local.Classes.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HRMS.Modules.DBModel.Local;

namespace Local.Models.Employee
{
    [DisplayName("Training")]
    [LocalTable("EmpTrainings", typeof(EmpTraining))]
    [EmpGeneralIDProperty("EmpID")]
    public class EmpTrainingModel : EducationModel
    {
        [LocalProperty("EmpEduType")]
        //[RequirePermission(Permission._EDUCATION)]
        [DisplayName("Training Type")]
        public new int? Type { get; set; }
    }
}
