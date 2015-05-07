using System;

namespace EffectFramework.Core.Models.Db
{
    public class ItemRecord
    {
        public int? ItemRecordID { get; set; }
        public int ItemID { get; set; }
        public int? EventID { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public bool IsDeleted { get; set; }
        public Guid Guid { get; set; }

        public Item Item { get; set; }
    }
}
