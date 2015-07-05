using EffectFramework.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace EffectFramework.Core.Services
{
    public static class CacheUtility
    {
        public static string GetCacheString<T>(params object[] Ids)
            where T : ICacheable
        {
            if (Ids == null)
            {
                throw new ArgumentNullException(nameof(Ids));
            }

            Type ObjectType = typeof(T);
            FieldInfo Info = ObjectType.GetField("CacheKeyFormatString");
            
            if (Info == null || !Info.IsLiteral)
            {
                throw new InvalidOperationException("Cacheable object does not have a constant CacheKeyFormString field.");
            }

            return string.Format((string)Info.GetRawConstantValue(), Ids);
        }
    }
}
