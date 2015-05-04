using HRMS.Core.Attributes;
using HRMS.Core.Security;
using HRMS.Modules.DBModel;
using HRMS.Modules.DBModel.Local;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.Employee
{
    [DisplayName("Emergency Contacts")]
    [LocalTable("EmpEmergencyContacts", typeof(EmpEmergencyContact))]
    [EmpGeneralIDProperty("EmpID")]
    public class EmergencyContactModel : Model
    {
        [LocalProperty("EmergencyContID")]
        //[RequirePermission(Permission._EMP_EM_CONTACT)]
        [DisplayName("Emergency ID")]
        [Key]
        //[SelfService]
        public int? EmergencyContactID { get; set; }

        [LocalProperty("ContactFName")]
        //[RequirePermission(Permission._EMP_EM_CONTACT)]
        [DisplayName("First Name")]
        [Required]
        //[SelfService]
        public string FirstName { get; set; }

        [LocalProperty("ContactLName")]
        //[RequirePermission(Permission._EMP_EM_CONTACT)]
        [DisplayName("Last Name")]
        [Required]
        //[SelfService]
        public string LastName { get; set; }

        [LocalProperty("Relationship")]
        //[RequirePermission(Permission._EMP_EM_CONTACT)]
        [Choices("Spouse", "Parent", "Child", "Friend", "Other")]
        [DisplayName("Relationship")]
        [Required]
        //[SelfService]
        public string Relationship { get; set; }

        [LocalProperty("Address1")]
        //[RequirePermission(Permission._EMP_EM_CONTACT)]
        [DisplayName("Address 1")]
        //[SelfService]
        public string Address1 { get; set; }

        [LocalProperty("Address2")]
        //[RequirePermission(Permission._EMP_EM_CONTACT)]
        [DisplayName("Address 2")]
        //[SelfService]
        public string Address2 { get; set; }

        [LocalProperty("City")]
        //[RequirePermission(Permission._EMP_EM_CONTACT)]
        [DisplayName("City")]
        //[SelfService]
        public string City { get; set; }

        [LocalProperty("StateProv")]
        //[RequirePermission(Permission._EMP_EM_CONTACT)]
        [DisplayName("State / Prov")]
        //[SelfService]
        public string State { get; set; }

        [LocalProperty("PostalCode")]
        //[RequirePermission(Permission._EMP_EM_CONTACT)]
        [DisplayName("Postal Code")]
        //[SelfService]
        public string PostalCode { get; set; }

        [LocalProperty("Country")]
        [CountryCode]
        //[RequirePermission(Permission._EMP_EM_CONTACT)]
        [Choices(typeof(HRMS.Core.Helpers.Countries), "ALL_COUNTRIES")]
        [DisplayName("Country")]
        //[SelfService]
        public string Country { get; set; }

        [LocalProperty("HomePhone")]
        //[RequirePermission(Permission._EMP_EM_CONTACT)]
        [DisplayName("Home Phone")]
        [CustomPhoneNumberValidate("Country")]
        //[SelfService]
        public string HomePhone { get; set; }

        [LocalProperty("WorkPhone")]
        //[RequirePermission(Permission._EMP_EM_CONTACT)]
        [DisplayName("Work Phone")]
        [CustomPhoneNumberValidate("Country")]
        //[SelfService]
        public string WorkPhone { get; set; }

        [LocalProperty("MobilePhone")]
        //[RequirePermission(Permission._EMP_EM_CONTACT)]
        [DisplayName("Mobile Phone")]
        [CustomPhoneNumberValidate("Country")]
        //[SelfService]
        public string MobilePhone { get; set; }

        //[LocalProperty("Pager")]
        //[RequirePermission(Permission._EMP_EM_CONTACT)]
        //[DisplayName("Pager")]
        //public string Pager { get; set; }

        [LocalProperty("Email")]
        //[RequirePermission(Permission._EMP_EM_CONTACT)]
        [EmailAddress]
        [DisplayName("Email Address")]
        //[SelfService]
        public string Email { get; set; }
    }
}
