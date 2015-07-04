using EffectFramework.Core.Models.Fields;

namespace EffectFramework.Core.Models
{
    public class ValidationResult
    {
        public FieldBase Field { get; private set; }
        public string Message { get; private set; }

        public ValidationResult(FieldBase Field, string Message)
        {
            this.Field = Field;
            this.Message = Message;
        }
    }
}
