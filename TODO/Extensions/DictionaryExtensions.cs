using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue ValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value = default(TValue);
            dictionary.TryGetValue(key, out value);
            return value;
        }
    }
}