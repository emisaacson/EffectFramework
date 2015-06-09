using System;
using System.Text.RegularExpressions;

namespace EffectFramework.Core.Models.Fields
{
    [Serializable]
    public class FieldTypeMetaText : FieldTypeMetaBase, IFieldTypeMeta
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
        public override bool HasRegex
        {
            get
            {
                return this.TextRegex != null;
            }
        }

        public FieldTypeMetaText()
            : base()
        {

        }

        public FieldTypeMetaText(bool IsRequired, Regex TextRegex)
        {
            this.IsRequired = IsRequired;
            this.TextRegex = TextRegex;
        }
        public FieldTypeMetaText(Db.FieldTypeMeta DbFieldTypeMeta)
            : base(DbFieldTypeMeta)
        { }
    }
}
