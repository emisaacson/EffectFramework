namespace EffectFramework.Core.Models.Fields
{
    /// <summary>
    /// A class representing a single ID/value pair from the lookup
    /// table of the persistence store.
    /// </summary>
    public class LookupEntry
    {
        public int ID { get; private set; }
        public string Value { get; private set; }

        public LookupEntry(int ID, string Value)
        {
            this.ID = ID;
            this.Value = Value;
        }
    }
}
