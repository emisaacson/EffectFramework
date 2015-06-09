using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Services
{
    public class NullCacheService : ICacheService
    {
        public string ConnectionString
        {
            set
            {

            }
        }

        public void DeleteObject(string Key)
        {
            
        }

        public object GetObject(string Key)
        {
            return null;
        }

        public bool? GetObjectAsBool(string Key)
        {
            return null;
        }

        public byte[] GetObjectAsByteArray(string Key)
        {
            return null;
        }

        public double? GetObjectAsDouble(string Key)
        {
            return null;
        }

        public int? GetObjectAsInt(string Key)
        {
            return null;
        }

        public long? GetObjectAsLong(string Key)
        {
            return null;
        }

        public string GetObjectAsString(string Key)
        {
            return null;
        }

        public void StoreObject(string Key, object Value)
        {
            
        }
    }
}
