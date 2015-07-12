using System;

namespace EffectFramework.Core.Models.Db
{
    public class Field
    {
        public long FieldID { get; set; }
        public long FieldTypeID { get; set; }
        public long EntityID { get; set; }
        public string ValueText { get; set; }
        public DateTime? ValueDate { get; set; }
        public Decimal? ValueDecimal { get; set; }
        public bool? ValueBoolean { get; set; }
        public long? ValueLookup { get; set; }
        public long? ValueItemReference { get; set; }
        public long? ValueEntityReference { get; set; }
        public byte[] ValueBinary { get; set; }
        public bool IsDeleted { get; set; }
        public Guid Guid { get; set; }
        public DateTime CreateDate { get; set; }
        public long? CreateItemReference { get; set; }
        public string CreateComment { get; set; }
        public DateTime? DeleteDate { get; set; }
        public long? DeleteItemReference { get; set; }
        public string DeleteItemComment { get; set; }
        public long TenantID { get; set; }

        public Entity Entity { get; set; }
        public Lookup Lookup { get; set; }
    }
}
