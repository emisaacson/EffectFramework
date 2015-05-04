using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.ViewModels
{
    public class ShowHiddenFieldsViewModel : Model
    {
        [Required]
        public string Tab { get; set; }
        [Required]
        public string Action { get; set; }
        [Required]
        public int EmpMasterId { get; set; }
    }
}