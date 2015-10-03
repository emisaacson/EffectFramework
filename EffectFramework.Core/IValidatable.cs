using EffectFramework.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core
{
    public interface IValidatable
    {
        ValidationSummary Validate();
    }

}
