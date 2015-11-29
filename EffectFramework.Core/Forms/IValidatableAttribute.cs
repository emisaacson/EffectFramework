using EffectFramework.Core.Models;
using EffectFramework.Core.Models.Fields;

namespace EffectFramework.Core.Forms
{
    public interface IValidatableAttribute
    {
        ValidationSummary Validate(object Value, FieldBase Field = null);
    }
}
