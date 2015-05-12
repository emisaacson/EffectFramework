using System;

namespace EffectFramework.Core.Forms
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class EffectiveDateAttribute : Attribute
    {
        public string FieldName { get; set; }
        public EffectiveDateAttribute() { }
    }
}
