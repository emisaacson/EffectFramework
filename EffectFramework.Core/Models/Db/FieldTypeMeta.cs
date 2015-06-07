using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models.Db
{
    public class FieldTypeMeta
    {
        public int FieldTypeMetaID { get; set; }
        public int ItemTypeID { get; set; }
        public int EntityTypeID { get; set; }
        public int FieldTypeID { get; set; }
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
    }
}
