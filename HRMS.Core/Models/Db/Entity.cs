using System;
using System.Collections.Generic;

namespace HRMS.Core.Models.Db
{
    public class Entity
    {
        public int EntityID { get; set; }
        public int EntityTypeID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public bool IsDeleted { get; set; }

        public Employee Employee { get; set; }
        public List<EntityField> EntityFields { get; set; }
    }
}
