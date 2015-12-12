using EffectFramework.Core.Models.Fields;
using EffectFramework.Core.Forms;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// A class to store the details of a single invalid field
    /// </summary>
    public class ValidationResult
    {

        // TODO: Make this cleaner

        private FieldBase _Field = null;
        private Form _Form = null;
        public string Field { get; private set; }
        public string Message { get; private set; }

        public ValidationResult(Form Form, FieldBase Field, string Message)
        {
            this._Field = Field;
            this._Form = Form;
            this.Message = Message;
        }

        public ValidationResult(Form Form, string Field, string Message)
        {
            this.Field = Field;
            this._Form = Form;
            this.Message = Message;
        }
        public ValidationResult(FieldBase Field, string Message)
        {
            this._Field = Field;
            this.Message = Message;
        }

        public ValidationResult(string Field, string Message)
        {
            this.Field = Field;
            this.Message = Message;
        }

        public ValidationResult(string Message)
        {
            this.Message = Message;
        }

        public FieldBase GetField()
        {
            return _Field;
        }

        public Form GetForm()
        {
            return _Form;
        }
    }
}
