using System;

namespace EffectFramework.Core.Forms
{
    /// <summary>
    /// Configures binding for a Form or its properties. Only properties with
    /// this attribute present will be bound. Global defaults should be added to the
    /// Form class and overrides and be specified on the Field or Property leve.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class BindAttribute : Attribute
    {
        public Type ItemType { get; private set; }
        public Type EntityType { get; private set; }
        public string FieldType { get; private set; }
        public string IDPropertyName { get; private set; }


        public BindAttribute(Type ItemType, Type EntityType, string FieldType, string IDPropertyName = null)
        {
            this.ItemType = ItemType;
            this.EntityType = EntityType;
            this.FieldType = FieldType;
            this.IDPropertyName = IDPropertyName;
        }

        public BindAttribute(Type ItemType, Type EntityType, string IDPropertyName = null)
        {
            this.ItemType = ItemType;
            this.EntityType = EntityType;
            this.IDPropertyName = IDPropertyName;
        }

        public BindAttribute(string FieldType, string IDPropertyName = null)
        {
            this.FieldType = FieldType;
            this.IDPropertyName = IDPropertyName;
        }

        public BindAttribute()
        {
        }
    }
}
