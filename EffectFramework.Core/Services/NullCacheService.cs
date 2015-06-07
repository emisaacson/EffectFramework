using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Services
{
    public class NullCacheService : ICacheService
    {
        public object GetObjectByKey(object Key)
        {
            return null;
        }

        public void StoreObject(object Key, object Value)
        {
            
        }
    }
}
