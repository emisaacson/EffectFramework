using HRMS.Modules.DBModel.Local;
using HRMS.Core.Attributes;
using HRMS.Core.Security;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Web;

namespace HRMS.Core.Models.Employee
{
    [DisplayName("Attachments")]
    [LocalTable("EmpAttachments", typeof(EmpAttachment))]
    [EmpGeneralIDProperty("EmpID")]
    [DataContract]
    public class AttachmentModel : Model, IValidatableObject
    {
        [LocalProperty("AttachID")]
        //[RequirePermission(Permission._EMP_ATTACH)]
        [DisplayName("Attachment ID")]
        [Key]
        //[SelfService]
        [DataMember]
        public int? AttachmentID { get; set; }

        [LocalProperty("AttachmentType")]
        //[RequirePermission(Permission._EMP_ATTACH)]
        [LocalReference("EmpAttachmentTypes", typeof(EmpAttachmentType), "AttachTypeID", "TypeName", new string[] { "IsActive", "Y" })]
        [DisplayName("Attachment Type")]
        [Required]
        [DataMember]
        //[SelfService]
        public int? AttachmentType { get; set; } // This property may come null from the client, thus it is marked as nullable

        [LocalProperty("AttachmentName")]
        //[RequirePermission(Permission._EMP_ATTACH)]
        [DisplayName("Name")]
        [MaxLength(260)]
        [Required]
        [DataMember]
        //[SelfService]
        public string Name { get; set; }

        [LocalProperty("AttachmentDesc")]
        //[RequirePermission(Permission._EMP_ATTACH)]
        [Textarea]
        [DisplayName("Description")]
        [DataMember]
        //[SelfService]
        public string Description { get; set; }

        //[RequirePermission(Permission._EMP_ATTACH)]
        [DataMember]
        //[SelfService]
        public string ClientID { get; set; } // This property maps attachments to posted files

        //[RequirePermission(Permission._EMP_ATTACH)]
        [JsonIgnore]
        //[SelfService]
        public HttpPostedFile File { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!AttachmentID.HasValue && File == null) // If new attachment without file
                yield return new ValidationResult("File uploads are required for new attachments.");
        }
    }
}
