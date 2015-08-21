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
        public Type ItemType { get; protected set; }
        public Type EntityType { get; protected set; }
        public string FieldType { get; protected set; }
        public string IDPropertyName { get; protected set; }
        public bool? UseStrictTypeChecking { get; protected set; }


        public BindAttribute(Type ItemType, Type EntityType, string FieldType, string IDPropertyName = null, bool? UseStrictTypeChecking = null)
        {
            this.ItemType = ItemType;
            this.EntityType = EntityType;
            this.FieldType = FieldType;
            this.IDPropertyName = IDPropertyName;
            this.UseStrictTypeChecking = UseStrictTypeChecking;
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
