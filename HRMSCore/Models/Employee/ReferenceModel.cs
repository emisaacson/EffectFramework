using HRMS.Modules.DBModel.Local;
using Local.Classes.Attributes;
using Local.Classes.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Local.Models.Employee
{
    [DisplayName("References")]
    [LocalTable("EmpReferences", typeof(EmpReference))]
    [EmpGeneralIDProperty("EmpID")]
    public class ReferenceModel : Model
    {
        [LocalProperty("EmpReferenceID")]
        //[RequirePermission(Permission._EMP_REFERENCES)]
        [DisplayName("Reference ID")]
        [Key]
        public int? ReferenceID { get; set; }

        [LocalProperty("FirstName")]
        //[RequirePermission(Permission._EMP_REFERENCES)]
        [DisplayName("First Name")]
        [MaxLength(64)]
        [Required]
        public string FirstName { get; set; }

        [LocalProperty("LastName")]
        //[RequirePermission(Permission._EMP_REFERENCES)]
        [DisplayName("Last Name")]
        [MaxLength(64)]
        [Required]
        public string LastName { get; set; }

        [LocalProperty("Relationship")]
        //[RequirePermission(Permission._EMP_REFERENCES)]
        [Choices("Manager", "Coworker", "Employee", "Family Member", "Friend", "Other", "Teacher")]
        [DisplayName("Relationship")]
        [MaxLength(64)]
        [Required]
        public string Relationship { get; set; }

        string countryCode;
        [CountryCode]
        //[RequirePermission(Permission._EMP_CONTACT)]
        [DisplayName("Country")]
        [Choices(typeof(Local.Classes.Helpers.Countries), "ALL_COUNTRIES")]
        public string CountryCode
        {
            get
            {
                if (String.IsNullOrEmpty(countryCode))
                {
                    PhoneNumbers.PhoneNumber PN;
                    if (!String.IsNullOrEmpty(PhoneNumber))
                    {
                        PN = PhoneNumbers.PhoneNumberUtil.GetInstance().Parse(PhoneNumber, "US");
                        return PhoneNumbers.PhoneNumberUtil.GetInstance().GetRegionCodeForNumber(PN);
                    }
                    else if (!String.IsNullOrEmpty(AlternateNumber))
                    {
                        PN = PhoneNumbers.PhoneNumberUtil.GetInstance().Parse(AlternateNumber, "US");
                        return PhoneNumbers.PhoneNumberUtil.GetInstance().GetRegionCodeForNumber(PN);
                    }
                }
                return countryCode;
            }

            set
            {
                countryCode = value;
            }
        }

        [LocalProperty("PhoneNumber")]
        //[RequirePermission(Permission._EMP_REFERENCES)]
        [CustomPhoneNumberValidate("CountryCode")]
        [DisplayName("Phone Number")]
        [MaxLength(32)]
        public string PhoneNumber { get; set; }

        [LocalProperty("AlternateNumber")]
        //[RequirePermission(Permission._EMP_REFERENCES)]
        [DisplayName("Alternate Number")]
        [CustomPhoneNumberValidate("CountryCode")]
        [MaxLength(32)]
        public string AlternateNumber { get; set; }

        [LocalProperty("EmailAddress")]
        //[RequirePermission(Permission._EMP_REFERENCES)]
        [DisplayName("Email Address")]
        [EmailAddress]
        [MaxLength(64)]
        public string EmailAddress { get; set; }
    }
}
