using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using HRMS.Core;
using HRMS.Core.Models.Db;

namespace HRMS.Test
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class DbContextTests
    {
        [Fact]
        public void TestCreateContext()
        {
            using (var db = new HrmsDbContext())
            {
                Console.Out.WriteLine(db.Database.ToString());
                Assert.Equal(db.Database.ToString(), "test");
            }
        }
    }
}
