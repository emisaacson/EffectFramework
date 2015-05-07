using System;
using System.Collections.Generic;

namespace EffectFramework.Core.Models.Db
{
    public class Item
    {
        public int? ItemID { get; set; }
        public int? ItemRecordID { get; set; }
        public bool IsDeleted { get; set; }
        public string DisplayName { get; set; }
        public Guid Guid { get; set; }

        public List<ItemRecord> ItemRecords { get; set; }
    }
}
