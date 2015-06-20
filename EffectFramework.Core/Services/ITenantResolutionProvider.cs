using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Services
{
    public interface ITenantResolutionProvider
    {
        int GetTenantID();
        string GetTenantDatabase();
    }
}
