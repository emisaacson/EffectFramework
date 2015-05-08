using HRMS.Core.Attributes;
using HRMS.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Core.Models.Employee
{
    public class AddressCollection : ModelCollection<AddressModel>
    {
        //[RequirePermission(Permission._EMP_ADDRESSES)]
        public override IEnumerable<AddressModel> Items { get; set; }

        public Dictionary<int, string> AddressTypes { get; set; }
    }
}