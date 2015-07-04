namespace EffectFramework.Core.Models.Fields
{
    public interface IFieldFilter
    {
        bool MaybeAllowSetValue(FieldBase Field, out object NewValue);
    }
}
