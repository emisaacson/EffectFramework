using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Core.Models.Entities
{
    public interface IEntity
    {
        EntityType Type { get; }
        int? EntityID { get; }
        int? EmployeeID { get; }
    }
}
