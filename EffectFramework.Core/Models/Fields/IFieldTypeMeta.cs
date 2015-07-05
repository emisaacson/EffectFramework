using System.Text.RegularExpressions;

namespace EffectFramework.Core.Models.Fields
{
    public interface IFieldTypeMeta : ICacheable
    {
        FieldBase Field { get; set; }
        bool HasRange { get; }
        bool HasRegex { get; }
        bool IsRequired { get; }
        Regex TextRegex { get; }
        object RangeMin { get; }
        object RangeMax { get; }
        FieldTypeMetaSafetyContainer GetSafetyContainer();
    }
}
