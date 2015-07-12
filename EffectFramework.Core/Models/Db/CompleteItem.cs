using System;

namespace EffectFramework.Core.Models.Db
{
    public class CompleteItem
    {
        public long ItemID { get; set; }
        public Guid ItemGuid { get; set; }
        public long ItemTenantID { get; set; }
        public long ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public long ItemTypeTenantID { get; set; }
        public long EntityID { get; set; }
        public long EntityTypeID { get; set; }
        public string EntityTypeName { get; set; }
        public long EntityTypeTenantID { get; set; }
        public DateTime EntityEffectiveDate { get; set; }
        public DateTime? EntityEndEffectiveDate { get; set; }
        public Guid EntityGuid { get; set; }
        public long EntityTenantID { get; set; }
        public long FieldTypeID { get; set; }
        public string FieldTypeName { get; set; }
        public long FieldTypeTenantID { get; set; }
        public long DataTypeID { get; set; }
        public string DataTypeName { get; set; }
        public long FieldID { get; set; }
        public string ValueText { get; set; }
        public DateTime? ValueDate { get; set; }
        public decimal? ValueDecimal { get; set; }
        public bool? ValueBoolean { get; set; }
        public long? ValueLookup { get; set; }
        public long? ValueItemReference { get; set; }
        public long? ValueEntityReference { get; set; }
        public byte[] ValueBinary { get; set; }
        public string LookupText { get; set; }
        public Guid EntityFieldGuid { get; set; }
        public long FieldTenantID { get; set; }
    }
}
