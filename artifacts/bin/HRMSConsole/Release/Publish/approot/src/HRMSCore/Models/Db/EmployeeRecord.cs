using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Core.Models.Db
{
    public class EmployeeRecord
    {
        public int EmployeeRecordID { get; set; }
        public int EmployeeID { get; set; }
        public int EventID { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime EndEffectiveDate { get; set; }
        public bool IsDeleted { get; set; }

        public Employee Employee { get; set; }
        public List<Entity> Entities { get; set; }
    }
}
