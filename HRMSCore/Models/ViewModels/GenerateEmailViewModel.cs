using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Local.Models.ViewModels
{
    public class GenerateEmailViewModel
    {
        public int empMasterId { get; set; }
        public string template { get; set; }
    }
}