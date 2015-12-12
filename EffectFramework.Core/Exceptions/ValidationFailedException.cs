using EffectFramework.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EffectFramework.Core.Exceptions
{
    public class ValidationFailedException : Exception
    {
        private ValidationSummary _ValidationSummary;
        public ValidationSummary ValidationSummary {
            get {
                if (_ValidationSummary == null && _ValidationMessage != null)
                {
                    List<ValidationResult> Errors = new List<ValidationResult>()
                    {
                        new ValidationResult(_ValidationMessage)
                    };
                    return new ValidationSummary(Errors);
                }
                return _ValidationSummary;
            }
            private set
            {
                _ValidationSummary = value;
            }
        }

        public string _ValidationMessage;
        public string ValidationMessage {
            get
            {
                if (_ValidationSummary != null)
                {
                    var Errors = ValidationSummary.Errors.Select(e => e.Message);

                    return string.Join("\n", Errors);
                }
                return _ValidationMessage;
            }
            private set
            {
                _ValidationMessage = value;
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
