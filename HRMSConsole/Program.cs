using System;
using System.Linq;
using HRMS.Core.Models.Db;

namespace HRMSConsole
{
    public class Program
    {
        public void Main(string[] args)
        {
            using (var db = new HrmsDbContext())
            {
                Employee NewEmployee = new Employee()
                {
                    DisplayName = "Test",
                    IsDeleted = false
                };

                db.Employees.Add(NewEmployee);
                db.SaveChanges();

                var x = db.Employees.Count();

                Employee EmployeeRemove = db.Employees.Where(z => z.EmployeeID == NewEmployee.EmployeeID && NewEmployee.IsDeleted == false).First();

                db.Employees.Remove(EmployeeRemove);
                db.SaveChanges();
            }
            Console.In.ReadLine();
        }
    }
}
