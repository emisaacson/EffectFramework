using System;

namespace EffectFramework.Core.Models.Fields
{
    [Serializable]
    public class FieldTypeMetaDecimal : FieldTypeMetaBase, IFieldTypeMeta
    {
        public decimal? RangeMax
        {
            get
            {
                return DecimalMax;
            }
        }

        public decimal? RangeMin
        {
            get
            {
                return DecimalMin;
            }
        }

        object IFieldTypeMeta.RangeMax
        {
            get
            {
                return ((FieldTypeMetaDecimal)this).RangeMax;
            }
        }

        object IFieldTypeMeta.RangeMin
        {
            get
            {
                return ((FieldTypeMetaDecimal)this).RangeMin;
            }
        }

        public override bool HasRange
        {
            get
            {
                return this.RangeMin.HasValue || this.RangeMax.HasValue;
            }
        }

        public FieldTypeMetaDecimal(bool IsRequired, decimal? Max, decimal? Min)
        {
            this._IsRequired = IsRequired;
            this.DecimalMax = Max;
            this.DecimalMin = Min;
        }

        public FieldTypeMetaDecimal() 
            : base()
        { }

        public FieldTypeMetaDecimal(Db.FieldTypeMeta DbFieldTypeMeta)
            : base(DbFieldTypeMeta)
        { }
    }
}
