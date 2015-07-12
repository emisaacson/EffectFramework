namespace EffectFramework.Core.Models.Db
{
    public class Tenant
    {
        public long TenantID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
