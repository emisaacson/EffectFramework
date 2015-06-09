using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models.Fields
{
    [Serializable]
    public class FieldTypeMetaDate : FieldTypeMetaBase, IFieldTypeMeta
    {

        public DateTime? RangeMax
        {
            get
            {
                return this.DateTimeMax;
            }
        }

        public DateTime? RangeMin
        {
            get
            {
                return this.DateTimeMin;
            }
        }

        object IFieldTypeMeta.RangeMax
        {
            get
            {
                return ((FieldTypeMetaDate)this).RangeMax;
            }
        }

        object IFieldTypeMeta.RangeMin
        {
            get
            {
                return ((FieldTypeMetaDate)this).RangeMin;
            }
        }

        public override bool HasRange
        {
            get
            {
                return this.RangeMin.HasValue || this.RangeMax.HasValue;
            }
        }

        public FieldTypeMetaDate() 
            : base()
        { }

        public FieldTypeMetaDate(bool IsRequired, DateTime? Max, DateTime? Min)
        {
            this.IsRequired = IsRequired;
            this.DateTimeMax = Max;
            this.DateTimeMin = Min;
        }

        public FieldTypeMetaDate(Db.FieldTypeMeta DbFieldTypeMeta)
            : base(DbFieldTypeMeta)
        { }
    }
}
