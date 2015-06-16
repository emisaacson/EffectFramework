using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models.Fields
{
    public interface IFieldTypeMeta
    {
        FieldBase Field { get; set; }
        bool HasRange { get; }
        bool HasRegex { get; }
        bool IsRequired { get; }
        Regex TextRegex { get; }
        object RangeMin { get; }
        object RangeMax { get; }
    }
}
