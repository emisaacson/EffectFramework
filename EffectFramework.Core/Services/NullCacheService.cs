using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Services
{
    public class NullCacheService : ICacheService
    {
        public object GetObjectByKey(string Key)
        {
            return null;
        }

        public void StoreObject(string Key, object Value)
        {
            
        }
    }
}
