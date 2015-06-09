using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using EffectFramework.Core.Models.Db;

namespace EffectFramework.Core.Models.Fields
{
    /// <summary>
    /// Base class for all FieldTypeMeta classes
    /// </summary>
    [Serializable]
    public class FieldTypeMetaBase
    {
        public bool IsRequired { get; protected set; }
        public Regex TextRegex { get; protected set; }

        protected decimal? DecimalMin { get; set; }
        protected decimal? DecimalMax { get; set; }
        protected DateTime? DateTimeMin { get; set; }
        protected DateTime? DateTimeMax { get; set; }

        public virtual bool HasRange {
            get {
                return false;
            }
        }
        public virtual bool HasRegex
        {
            get
            {
                return false;
            }
        }

        public FieldTypeMetaBase() {
            this.IsRequired = false;
        }

        public FieldTypeMetaBase(FieldTypeMeta DbFieldTypeMeta)
        {
            if (DbFieldTypeMeta != null)
            {
                // EITODO: Read queries and evaluate them
                this.IsRequired = DbFieldTypeMeta.IsRequired.HasValue && DbFieldTypeMeta.IsRequired.Value ? true : false;
                this.TextRegex = DbFieldTypeMeta.TextRegex != null ? new Regex(DbFieldTypeMeta.TextRegex) : null;
                this.DecimalMin = DbFieldTypeMeta.DecimalMin;
                this.DecimalMax = DbFieldTypeMeta.DecimalMax;
                this.DateTimeMin = DbFieldTypeMeta.DatetimeMin;
                this.DateTimeMax = DbFieldTypeMeta.DatetimeMax;
            }
            else
            {
                this.IsRequired = false;
            }
        }
    }
}
