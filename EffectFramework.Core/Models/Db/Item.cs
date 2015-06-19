using System;
using System.Collections.Generic;

namespace EffectFramework.Core.Models.Db
{
    public class Item
    {
        public int? ItemID { get; set; }
        public int ItemTypeID { get; set; }
        public bool IsDeleted { get; set; }
        public Guid Guid { get; set; }
        public int TenantID { get; set; }

        public List<Entity> Entities { get; set; }
    }
}
