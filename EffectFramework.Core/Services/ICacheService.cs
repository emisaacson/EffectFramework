namespace EffectFramework.Core.Services
{
    public interface ICacheService
    {
        object GetObject(string Key);
        byte[] GetObjectAsByteArray(string Key);
        long? GetObjectAsInt(string Key);
        bool? GetObjectAsBool(string Key);
        long? GetObjectAsLong(string Key);
        double? GetObjectAsDouble(string Key);
        string GetObjectAsString(string Key);
        void StoreObject(string Key, object Value);
        void DeleteObject(string Key);
        void Flush();
    }
}
