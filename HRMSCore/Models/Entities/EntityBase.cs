using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Core.Models.Entities
{
    public abstract class EntityBase
    {
        public abstract EntityType Type { get; }
        public int? EntityID { get; protected set; }
        public int? EmployeeID { get; protected set; }
        public void LoadUpEntity(Db.Entity DbEntity)
        {
            this.EntityID = DbEntity.EntityID;
            this.EmployeeID = DbEntity.EmployeeID;
        }
    }
}
