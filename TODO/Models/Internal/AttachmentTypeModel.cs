using HRMS.Modules.DBModel.Local;
using HRMS.Core.Attributes;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.Internal
{
    [DisplayName("Attachment Types")]
    [LocalTable("EmpAttachmentTypes", typeof(EmpAttachmentType))]
    public class AttachmentTypeModel : Model
    {
        [DisplayName("Attachment Type ID")]
        //[RequirePermission(Permission._EMP_BASIC)]
        [LocalProperty("AttachTypeID")]
        [Key]
        public int? AttachmentTypeID { get; set; }

        [DisplayName("Attachment Type")]
        //[RequirePermission(Permission._EMP_BASIC)]
        [LocalProperty("TypeName")]
        public string AttachmentTypeName { get; set; }
    }
}