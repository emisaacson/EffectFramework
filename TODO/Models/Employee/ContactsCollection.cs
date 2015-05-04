using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HRMS.Core.Security;
using HRMS.Core.Attributes;

namespace HRMS.Core.Models.Employee
{
    public class ContactsCollection : ModelCollection<ContactModel>
    {
        //[RequirePermission(Permission._EMP_CONTACT)]
        public override IEnumerable<ContactModel> Items { get; set; }

        public Dictionary<int, string> ContactTypes { get; set; }
    }
}