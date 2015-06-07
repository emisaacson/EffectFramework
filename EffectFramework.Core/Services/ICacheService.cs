using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Services
{
    public interface ICacheService
    {
        object GetObjectByKey(string Key);
        void StoreObject(string Key, object Value);
    }
}
