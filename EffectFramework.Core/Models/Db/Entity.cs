using System;
using System.Collections.Generic;

namespace EffectFramework.Core.Models.Db
{
    public class Entity
    {
        public int EntityID { get; set; }
        public int EntityTypeID { get; set; }
        public int ItemID { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public bool IsDeleted { get; set; }
        public Guid Guid { get; set; }

        public Item Item { get; set; }
        public List<EntityField> EntityFields { get; set; }
    }
}
