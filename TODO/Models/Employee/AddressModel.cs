using HRMS.Core.Attributes;
using HRMS.Core.Security;
using HRMS.Modules.DBModel.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Core.Models.Employee
{
    [DisplayName("Addresses")]
    [LocalTable("EmpAddresses", typeof(EmpAddress))]
    [EmpGeneralIDProperty("EmpGeneralID")]
    public class AddressModel : Model
    {
        [LocalProperty("EmpAddressID")]
        //[RequirePermission(Permission._EMP_ADDRESSES)]
        [DisplayName("Address ID")]
        [Key]
        //[SelfService(true)]
        public int? AddressID { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_ADDRESSES)]
        [Choices("1|Primary", "3|Secondary")]
        [DisplayName("Address Type")]
        [HideInCustomReports]
        [Required]
        //[SelfService]
        public int? AddressType { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_ADDRESSES)]
        [MaxLength(1024)]
        [Required]
        //[SelfService]
        public string Address1 { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_ADDRESSES)]
        [MaxLength(1024)]
        //[SelfService]
        public string Address2 { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_ADDRESSES)]
        [MaxLength(50)]
        //[SelfService]
        public string City { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_ADDRESSES)]
        [MaxLength(50)]
        //[SelfService]
        public string County { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_ADDRESSES)]
        [MaxLength(50)]
        //[SelfService]
        public string State { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_ADDRESSES)]
        [MaxLength(50)]
        [DisplayName("Postal Code")]
        [Required]
        //[SelfService]
        public string PostalCode { get; set; }

        [LocalProperty]
        //[RequirePermission(Permission._EMP_ADDRESSES)]
        [Choices(typeof(HRMS.Core.Helpers.Countries), "ALL_COUNTRIES")]
        [Required]
        //[SelfService]
        public string Country { get; set; }
    }
}
