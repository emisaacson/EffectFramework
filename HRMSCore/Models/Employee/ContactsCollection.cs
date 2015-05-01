using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Local.Classes.Security;
using Local.Classes.Attributes;

namespace Local.Models.Employee
{
    public class ContactsCollection : ModelCollection<ContactModel>
    {
        //[RequirePermission(Permission._EMP_CONTACT)]
        public override IEnumerable<ContactModel> Items { get; set; }

        public Dictionary<int, string> ContactTypes { get; set; }
    }
}