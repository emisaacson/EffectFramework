using Local.Classes.Attributes;
using Local.Classes.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Local.Models.Employee
{
    [DisplayName("Compliance")]
    public class ComplianceModel : Model
    {
        [MasterProperty]
        //[ReadOnly(true)]
        [DisplayName("Training")]
        //[RequirePermission(Permission._TRAINING)]
        [Key]
        public string TrainingName { get; set; }

        [MasterProperty]
        //[ReadOnly(true)]
        [DisplayName("Description")]
        //[RequirePermission(Permission._TRAINING)]
        public string TrainingDescription { get; set; }

        [MasterProperty]
        //[ReadOnly(true)]
        [DisplayName("Status")]
        //[RequirePermission(Permission._TRAINING)]
        public string StatusText { get; set; }

        [MasterProperty]
        //[ReadOnly(true)]
        [DisplayName("Start Date")]
        //[RequirePermission(Permission._TRAINING)]
        public DateTime StartDate { get; set; }

        [MasterProperty]
        //[ReadOnly(true)]
        [DisplayName("Due Date")]
        //[RequirePermission(Permission._TRAINING)]
        public DateTime DueDate { get; set; }

        [MasterProperty]
        //[ReadOnly(true)]
        [DisplayName("Score")]
        //[RequirePermission(Permission._TRAINING)]
        public string Score { get; set; }

        [MasterProperty]
        //[ReadOnly(true)]
        [DisplayName(" ")]
        //[RequirePermission(Permission._TRAINING)]
        public string Link { get; set; }
    }
}