using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models
{
    public class ValidationSummary
    {
        public bool IsValid { get; private set; }
        public IEnumerable<ValidationResult> Errors { get; private set; }
        public ValidationSummary(IEnumerable<ValidationResult> Errors)
        {
            if (Errors == null)
            {
                throw new ArgumentNullException(nameof(Errors));
            }
            this.Errors = Errors;
            if (this.Errors.Count() > 0)
            {
                this.IsValid = false;
            }
            else
            {
                this.IsValid = true;
            }
        }
    }
}
