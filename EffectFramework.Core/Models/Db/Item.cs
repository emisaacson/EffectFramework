using System;
using System.Collections.Generic;

namespace EffectFramework.Core.Models.Db
{
    public class Item
    {
        public long? ItemID { get; set; }
        public long ItemTypeID { get; set; }
        public bool IsDeleted { get; set; }
        public Guid Guid { get; set; }
        public long TenantID { get; set; }

        public List<Entity> Entities { get; set; }
    }
}
