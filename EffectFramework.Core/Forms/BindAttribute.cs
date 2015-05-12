using System;

namespace EffectFramework.Core.Forms
{
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
