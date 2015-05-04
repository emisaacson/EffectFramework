using HRMS.Core.Attributes;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace HRMS.Core.Models.Models
{
    [DataContract]
    [DisplayName("User")]

    public class UserModel : Model
    {
        [LocalProperty("UserID")]
        //[RequirePermission(Permission._SYSTEM_SETTINGS)]
        [DisplayName("User ID")]
        [Key]
        public int UserID { get; set; }

        [Required]
        [DataMember]
        [LocalProperty("ADUser")]
        //[RequirePermission(Permission._SYSTEM_SETTINGS)]
        [DisplayName("User Name")]
        public string UserName { get; set; }
        
        //[Required]
        //[PasswordInput]
        //[DataMember]
        //[LocalProperty]
        ////[RequirePermission(Permission._SYSTEM_SETTINGS)]
        //[DisplayName("Password")]
        //public string Password { get; set; }

        [Required]
        [LocalProperty]
        [DataMember]
        //[RequirePermission(Permission._SYSTEM_SETTINGS)]
        [DisplayName("Domain")]
        public string Domain { get; set; }

        [Textarea]
        [LocalProperty]
        [DataMember]
        //[RequirePermission(Permission._SYSTEM_SETTINGS)]
        [DisplayName("Notes")]
        public string Notes { get; set; }

        [LocalProperty]
        [DataMember]
        //[RequirePermission(Permission._SYSTEM_SETTINGS)]    
        public char IsActive { get; set; }




    }
}