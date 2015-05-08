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
    [DisplayName("Contact Types")]
    [LocalTable("ContactInfoTypes", typeof(ContactInfoType))]
    public class ContactTypeModel : Model
    {
        [DisplayName("Contact Type ID")]
        //[RequirePermission(Permission._EMP_CONTACT)]
        [LocalProperty("ContInfoTypeID")]
        [Key]
        public int? ContactTypeID { get; set; }

        [DisplayName("Contact Type")]
        //[RequirePermission(Permission._EMP_CONTACT)]
        [LocalProperty("TypeName")]
        public string ContactTypeName { get; set; }
    }
}