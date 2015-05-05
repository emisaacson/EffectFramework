using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Core.Models.Entities
{
    public class AddressEntity : EntityBase
    {
        public override EntityType Type
        {
            get
            {
                return EntityType.Address;
            }
        }
    }
}
