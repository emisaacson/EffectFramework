using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.ViewModels
{
    public class StartImpersonateViewModel : Model
    {
        public List<string> Domains { get; set; }

        public string SelectedDomain { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}