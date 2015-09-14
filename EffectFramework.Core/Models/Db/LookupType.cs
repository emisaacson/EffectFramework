using System;

namespace EffectFramework.Core.Models.Db
{
    public class LookupType
    {
        public long LookupTypeID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public Guid Guid { get; set; }
        public long TenantID { get; set; }
        public bool IsReadOnly { get; set; } = false;
        public bool IsHierarchical { get; set; } = false;
    }
}
