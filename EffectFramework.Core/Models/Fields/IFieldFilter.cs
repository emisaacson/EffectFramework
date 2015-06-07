using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models.Fields
{
    public interface IFieldFilter
    {
        bool MaybeAllowSetValue(FieldBase Field, out object NewValue);
    }
}
