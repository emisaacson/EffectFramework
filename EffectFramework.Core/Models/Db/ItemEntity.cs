using System;

namespace EffectFramework.Core.Models.Db
{
    public class ItemEntity
    {
        public int ItemEntityID { get; set; }
        public int ItemRecordID { get; set; }
        public int EntityID { get; set; }
        public bool IsDeleted { get; set; }
        public Guid Guid { get; set; }

    }
}
