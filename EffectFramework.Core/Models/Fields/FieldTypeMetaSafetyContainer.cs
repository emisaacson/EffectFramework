using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models.Fields
{
    public class FieldTypeMetaSafetyContainer
    {
        private FieldTypeMetaBase Meta;
        public FieldTypeMetaSafetyContainer(IFieldTypeMeta Template)
        {
            Type MetaType = Template.GetType();
            Meta = (FieldTypeMetaBase)Activator.CreateInstance(MetaType);
            Meta.CopyValuesFrom((FieldTypeMetaBase)Template);
        }
        public IFieldTypeMeta GetMeta(IField Field)
        {
            Type MetaType = Meta.GetType();
            var NewMeta = (FieldTypeMetaBase)Activator.CreateInstance(MetaType);
            NewMeta.CopyValuesFrom(Meta);
            NewMeta.Field = (FieldBase)Field;
            return (IFieldTypeMeta)NewMeta;
        }
    }
}
