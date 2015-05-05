using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Core.Models.Db;
using Xunit;

namespace HRMS.Test
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class DBContextTests
    {
        [Fact]
        public void CreateDBContext()
        {
            using (var db = new HrmsDbContext())
            {

            }
        }

        [Fact]
        public void CreateEmployee()
        {
            Employee Employee = new Employee();
            using (var db = new HrmsDbContext())
            {
                
            }
        }


    }
}
