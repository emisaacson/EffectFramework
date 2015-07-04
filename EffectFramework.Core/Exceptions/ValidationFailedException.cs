using EffectFramework.Core.Models;
using System;
using System.Linq;

namespace EffectFramework.Core.Exceptions
{
    public class ValidationFailedException : Exception
    {
        public ValidationSummary ValidationSummary { get; private set; }

        public string _ValidationMessage;
        public string ValidationMessage {
            get
            {
                if (ValidationSummary != null)
                {
                    var Errors = ValidationSummary.Errors.Select(e => e.Message);

                    return string.Join("\n", Errors);
                }
                return _ValidationMessage;
            }
            private set
            {
                this._ValidationMessage = value;
            }
        }

        public ValidationFailedException(ValidationSummary ValidationSummary)
        {
            this.ValidationSummary = ValidationSummary;
        }

        public ValidationFailedException(string Message)
        {
            this.ValidationMessage = Message;
        }
    }
}
