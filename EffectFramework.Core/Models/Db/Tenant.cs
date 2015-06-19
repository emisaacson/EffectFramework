using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models.Db
{
    public class Tenant
    {
        public int TenantID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
