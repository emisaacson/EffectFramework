using System;


namespace EffectFramework.Core.Models.Fields
{
    [Serializable]
    public class FieldTypeMetaBasic : FieldTypeMetaBase, IFieldTypeMeta
    {
        public object RangeMax
        {
            get
            {
                return null;
            }
        }

        public object RangeMin
        {
            get
            {
                return null;
            }
        }

        public FieldTypeMetaBasic() 
            : base()
        { }

        public FieldTypeMetaBasic(bool IsRequired)
        {
            this.IsRequired = IsRequired;
        }

        public FieldTypeMetaBasic(Db.FieldTypeMeta DbFieldTypeMeta)
            : base(DbFieldTypeMeta)
        { }
    }
}
