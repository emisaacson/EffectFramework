using HRMS.Modules.Training;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.ViewModels
{
    public class PDFTrainingRendererViewModel
    {
        public string FilePath { get; set; }
        public TrainingAssignment TrainingAssignment { get; set; }
    }
}