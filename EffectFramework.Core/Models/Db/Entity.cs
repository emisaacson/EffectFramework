using System;
using System.Collections.Generic;

namespace EffectFramework.Core.Models.Db
{
    public class Entity
    {
        public long EntityID { get; set; }
        public long EntityTypeID { get; set; }
        public long ItemID { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public bool IsDeleted { get; set; }
        public Guid Guid { get; set; }
        public DateTime CreateDate { get; set; }
        public long? CreateItemReference { get; set; }
        public string CreateComment { get; set; }
        public DateTime? DeleteDate { get; set; }
        public long? DeleteItemReference { get; set; }
        public string DeleteItemComment { get; set; }
        public long TenantID { get; set; }

        public Item Item { get; set; }
        public List<Field> EntityFields { get; set; }
    }
}
