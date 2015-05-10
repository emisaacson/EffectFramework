using System;

namespace EffectFramework.Core.Models.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class RequiredAttribute : Attribute
    {
    }
}
