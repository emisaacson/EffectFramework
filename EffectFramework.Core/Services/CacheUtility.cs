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
        /// <summary>
        /// Gets the cache key for an object that implements ICacheable interface,
        /// and also provides a constant CacheKeyFormatString field.
        /// </summary>
        /// <typeparam name="T">The type of object to get a cache key for</typeparam>
        /// <param name="Ids">One of more unique identifiers to pass to the format string.</param>
        /// <returns>A unique cache key for the object.</returns>
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
                throw new InvalidOperationException("Cacheable object does not have a constant CacheKeyFormatString field.");
            }

            return string.Format((string)Info.GetRawConstantValue(), Ids);
        }
    }
}
