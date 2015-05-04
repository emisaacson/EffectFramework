using HRMS.Core.Models.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.ViewModels
{
    public class SendEmailViewModel : Model
    {
        public string MainRecipientEmail { get; set; }
        [Required]
        public int EmpMasterId { get; set; }

        public List<EmailTemplateModel> Templates { get; set; }

        public List<string> SourceEmails { get; set; }
        [Required]
        public List<string> To { get; set; }
        public List<string> CC { get; set; }
        public List<string> BCC { get; set; }
        public string Subject { get; set; }

        public string Template { get; set; }

        public string FilledEMail { get; set; }

        public string MainRecipientFullName { get; set; }

        public List<string> AvailableModelProperies { get; set; }

    }
}