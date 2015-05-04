using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMSConsole
{
    public class Program
    {
        public void Main(string[] args)
        {
            using (var db = new HRMS.Core.Models.Db.HrmsDbContext())
            {
                Console.Out.WriteLine(db.Database.ToString());
            }
                Console.In.ReadLine();
        }
    }
}
