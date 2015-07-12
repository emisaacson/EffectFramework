using System;

namespace EffectFramework.Core.Models.Db
{
    public class FieldTypeMeta
    {
        public long FieldTypeMetaID { get; set; }
        public long ItemTypeID { get; set; }
        public long EntityTypeID { get; set; }
        public long FieldTypeID { get; set; }
        public bool? IsRequired { get; set; }
        public string IsRequiredQuery { get; set; }
        public decimal? DecimalMin { get; set; }
        public string DecimalMinQuery { get; set; }
        public decimal? DecimalMax { get; set; }
        public string DecimalMaxQuery { get; set; }
        public DateTime? DatetimeMin { get; set; }
        public string DatetimeMinQuery { get; set; }
        public DateTime? DatetimeMax { get; set; }
        public string DatetimeMaxQuery { get; set; }
        public string TextRegex { get; set; }
        public string TextRegexQuery { get; set; }
        public long TenantID { get; set; }

    }
}
