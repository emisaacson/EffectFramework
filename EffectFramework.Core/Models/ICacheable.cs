using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models
{
    public interface ICacheable
    {
        string GetCacheKey();
    }
}
