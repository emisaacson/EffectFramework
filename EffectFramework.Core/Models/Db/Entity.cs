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
        public DateTime CreateDate { get; set; }
        public int? CreateItemReference { get; set; }
        public string CreateComment { get; set; }
        public DateTime? DeleteDate { get; set; }
        public int? DeleteItemReference { get; set; }
        public string DeleteItemComment { get; set; }

        public Item Item { get; set; }
        public List<Field> EntityFields { get; set; }
    }
}
