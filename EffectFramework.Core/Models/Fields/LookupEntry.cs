using System;

namespace EffectFramework.Core.Models.Fields
{
    /// <summary>
    /// A class representing a single ID/value pair from the lookup
    /// table of the persistence store.
    /// </summary>
    [Serializable]
    public class LookupEntry
    {
        public int ID { get; private set; }
        public string Value { get; private set; }
        public int TenantID { get; private set; }

        public LookupEntry(int ID, string Value, int TenantID)
        {
            this.ID = ID;
            this.Value = Value;
            this.TenantID = TenantID;
        }
    }
}
