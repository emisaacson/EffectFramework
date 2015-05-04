using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.ViewModels
{
    public class CreateEditEmailTemplate
    {
        public string Html { get; set; }
        public int EmployeId { get; set; }
        public int EmailTemplateId { get; set; }

        [Required]
        [DisplayName("Domain")]
        public string TemplateName { get; set; }
        

    }
}