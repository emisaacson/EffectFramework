using System;

namespace EffectFramework.Core.Forms
{

    /// <summary>
    /// Attribute to specify the field to use as an EffectiveDate on a Form class.
    /// 
    /// Specify FieldName if used on the class. This will indicate all class members default
    /// to the specified field name.
    /// 
    /// You can also attach directly to the member that should receive the EffectiveDate binding
    /// and it will also be considered a global default.
    /// 
    /// Attach to any other member with a FieldName specified and it will override the default
    /// for that member only.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class EffectiveDateAttribute : Attribute
    {
        public string FieldName { get; set; }
        public EffectiveDateAttribute() { }
    }
}
