using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Core.Models.Db
{
    public class EmployeeEntity
    {
        public int EmployeeEntityID { get; set; }
        public int EmployeeRecordID { get; set; }
        public int EntityID { get; set; }
        public bool IsDeleted { get; set; }

    }
}
