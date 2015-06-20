using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Services
{
    public class DefaultTenantResolver : ITenantResolutionProvider
    {
        public string GetTenantDatabase()
        {
            return "HRMS";
        }

        public int GetTenantID()
        {
            return 1;
        }
    }
}
