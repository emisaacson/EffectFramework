using System;

namespace EffectFramework.Core.Models.Db
{
    public class AuditLog
    {
        public long AuditLogID { get; set; }
        public long ItemID { get; set; }
        public long EntityID { get; set; }
        public long? FieldID { get; set; }
        public DateTime? EffectiveDateOld { get; set; }
        public DateTime? EffectiveDateNew { get; set; }
        public DateTime? EndEffectiveDateOld { get; set; }
        public DateTime? EndEffectiveDateNew { get; set; }
        public string ValueTextOld { get; set; }
        public string ValueTextNew { get; set; }
        public DateTime? ValueDateOld { get; set; }
        public DateTime? ValueDateNew { get; set; }
        public decimal? ValueDecimalOld { get; set; }
        public decimal? ValueDecimalNew { get; set; }
        public bool? ValueBooleanOld { get; set; }
        public bool? ValueBooleanNew { get; set; }
        public long? ValueLookupOld { get; set; }
        public long? ValueLookupNew { get; set; }
        public byte[] ValueBinaryOld { get; set; }
        public byte[] ValueBinaryNew { get; set; }
        public long? ValueItemReferenceOld { get; set; }
        public long? ValueItemReferenceNew { get; set; }
        public long? ValueEntityReferenceOld { get; set; }
        public long? ValueEntityReferenceNew { get; set; }
        public DateTime CreateDate { get; set; }
        public long? ItemReference { get; set; }
        public string Comment { get; set; }
        public long TenantID { get; set; }

    }
}
