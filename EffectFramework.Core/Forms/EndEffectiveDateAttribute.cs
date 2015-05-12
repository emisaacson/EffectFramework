using System;

namespace EffectFramework.Core.Forms
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class EndEffectiveDateAttribute : Attribute
    {
        public string FieldName { get; set; }
        public EndEffectiveDateAttribute() { }
    }
}
