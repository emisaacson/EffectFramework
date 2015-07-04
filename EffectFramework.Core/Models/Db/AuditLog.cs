using System;

namespace EffectFramework.Core.Models.Db
{
    public class AuditLog
    {
        public int AuditLogID { get; set; }
        public int ItemID { get; set; }
        public int EntityID { get; set; }
        public int? FieldID { get; set; }
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
        public int? ValueLookupOld { get; set; }
        public int? ValueLookupNew { get; set; }
        public byte[] ValueBinaryOld { get; set; }
        public byte[] ValueBinaryNew { get; set; }
        public int? ValueItemReferenceOld { get; set; }
        public int? ValueItemReferenceNew { get; set; }
        public int? ValueEntityReferenceOld { get; set; }
        public int? ValueEntityReferenceNew { get; set; }
        public DateTime CreateDate { get; set; }
        public int? ItemReference { get; set; }
        public string Comment { get; set; }
        public int TenantID { get; set; }

    }
}
