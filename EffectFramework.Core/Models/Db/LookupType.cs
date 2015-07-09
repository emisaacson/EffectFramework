using System;

namespace EffectFramework.Core.Models.Db
{
    public class LookupType
    {
        public int LookupTypeID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public Guid Guid { get; set; }
        public int TenantID { get; set; }
        public bool IsReadOnly { get; set; } = false;
    }
}
