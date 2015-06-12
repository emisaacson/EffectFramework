using EffectFramework.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core
{
    public class ValidationFailedException : Exception
    {
        public ValidationSummary ValidationSummary { get; private set; }

        public ValidationFailedException(ValidationSummary ValidationSummary)
        {
            this.ValidationSummary = ValidationSummary;
        }
    }
}
