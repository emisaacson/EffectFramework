namespace EffectFramework.Core.Models.Fields
{
    public class NullFieldFilter : IFieldFilter
    {
        public bool MaybeAllowSetValue(FieldBase Field, out object NewValue)
        {
            NewValue = ((IField)Field).Value;
            return true;
        }
    }
}
