using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HRMS.Core.Security;
using HRMS.Core.Attributes;
using System.ComponentModel;
using HRMS.Modules.DBModel.Local;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Core.Models.Employee
{
    [DisplayName("Contact Information")]
    [LocalTable("EmpContactInfos", typeof(EmpContactInfo))]
    [EmpGeneralIDProperty("EmpID")]
    public class ContactModel : Model
    {
        [LocalProperty("ContactInfoID")]
        //[RequirePermission(Permission._EMP_CONTACT)]
        [DisplayName("Contact ID")]
        [Key]
        //[SelfService]
        public int? ContactID { get; set; }

        [LocalProperty("ContactType")]
        //[RequirePermission(Permission._EMP_CONTACT)]
        [DisplayName("Contact Type")]
        [Choices("3|Home Phone",
                 "7|Personal Mobile Phone",
                 "8|Personal Email",
                 "9|Fax")]
        [Required]
        //[SelfService]
        public int? Type { get; set; }

        string countryCode;
        [CountryCode]
        //[RequirePermission(Permission._EMP_CONTACT)]
        [DisplayName("Country")]
        //[SelfService]
        [Choices(typeof(HRMS.Core.Helpers.Countries), "ALL_COUNTRIES")]
        public string CountryCode
        {
            get 
            {
                if (!String.IsNullOrEmpty(Data) && String.IsNullOrEmpty(countryCode))
                {
                    if (Type == 3 || Type == 7 || Type == 9)
                    {
                        PhoneNumbers.PhoneNumber PN = PhoneNumbers.PhoneNumberUtil.GetInstance().Parse(Data, "US");
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

        [LocalProperty("ContactData")]
        //[RequirePermission(Permission._EMP_CONTACT)]
        [DisplayName("Info")]
        [Required]
        [MaxLength(100)]
        //[SelfService]
        [CustomPhoneNumberValidate("CountryCode")]
        public string Data { get; set; }

        [LocalProperty("ContactData2")]
        //[RequirePermission(Permission._EMP_CONTACT)]
        [DisplayName("Secondary Info")]
        [MaxLength(50)]
        //[SelfService]
        public string Data2 { get; set; }
    }
}
