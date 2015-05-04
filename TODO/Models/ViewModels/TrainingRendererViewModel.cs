using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.ViewModels
{
    public class TrainingRendererViewModel
    {
        public string Title { get; set; }
        public string Viewer { get; set; }
        public string Form { get; set; }
        public List<string> Scripts { get; set; }
        public List<string> StyleSheets { get; set; }
    }
}