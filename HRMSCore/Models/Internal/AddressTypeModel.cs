using HRMS.Modules.DBModel.Local;
using Local.Classes.Attributes;
using Local.Classes.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Local.Models.Internal
{
    [DisplayName("Address Types")]
    [LocalTable("EmpAddressTypes", typeof(EmpAddressType))]
    public class AddressTypeModel : Model
    {
        [DisplayName("Address Type ID")]
        //[RequirePermission(Permission._EMP_ADDRESSES)]
        [LocalProperty]
        [Key]
        public int? AddressTypeID { get; set; }

        [DisplayName("Address Type")]
        //[RequirePermission(Permission._EMP_ADDRESSES)]
        [LocalProperty]
        public string AddressTypeName { get; set; }
    }
}