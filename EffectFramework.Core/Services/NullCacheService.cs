namespace EffectFramework.Core.Services
{
    /// <summary>
    /// Takes your objects and drops them on the floor
    /// </summary>
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

        public long? GetObjectAsInt(string Key)
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

        public void Flush()
        {

        }
    }
}
