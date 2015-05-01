using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Local.Models.Employee
{
    // Jquery Datatables (used on the main employee list, users list, roles list)
    // needs an ajax response that looks like this.
    public class JqDataTablesEmployeeDataTable
    {
        public IEnumerable<List<string>> aaData;
    }
}