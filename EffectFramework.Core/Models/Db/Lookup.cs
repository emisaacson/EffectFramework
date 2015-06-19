namespace EffectFramework.Core.Models.Db
{
    public class Lookup
    {
        public int LookupID { get; set; }
        public string Value { get; set; }
        public int LookupTypeID { get; set; }
        public bool IsDeleted { get; set; }
        public int TenantID { get; set; }
    }
}
