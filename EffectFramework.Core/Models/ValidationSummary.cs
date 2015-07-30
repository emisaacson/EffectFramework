﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// Class to store validation result of an item.
    /// </summary>
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
