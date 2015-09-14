using System;

namespace EffectFramework.Core.Models.Db
{
    public class Lookup
    {
        public long LookupID { get; set; }
        public string Value { get; set; }
        public long LookupTypeID { get; set; }
        public bool IsDeleted { get; set; }
        public Guid Guid { get; set; }
        public long TenantID { get; set; }
        public long? ParentID { get; set; }
    }
}
