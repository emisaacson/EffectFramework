using System;
using System.Linq;
using System.Reflection;
using EffectFramework.Core.Models;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Models.Fields;

namespace EffectFramework.Core.Forms
{
    public abstract class Form
    {
        protected Item BoundItem { get; set; }
        protected EntityBase BoundEntity { get; set; }
        public void BindTo(Item Item)
        {
            this.BoundItem = Item;
        }

        public void Populate()
        {
            if (this.BoundItem == null)
            {
                throw new InvalidOperationException("Must bind the form to an item first.");
            }

            // Get form global bindings
            Type TypeOfForm = this.GetType();
            var FormBinding = TypeOfForm.GetCustomAttribute<BindAttribute>();
            Type FormItemType = null;
            Type FormEntityType = null;
            string FormIDPropertyName = null;


            if (FormBinding != null)
            {
                FormItemType = FormBinding.ItemType;
                FormEntityType = FormBinding.EntityType;
                FormIDPropertyName = FormBinding.IDPropertyName;
            }

            // Get Property and field bindings
            var AllProperties = TypeOfForm.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var Property in AllProperties)
            {
                var PropertyBinding = Property.GetCustomAttribute<BindAttribute>();
                if (PropertyBinding != null)
                {
                    var PropertyItemType = PropertyBinding.ItemType ?? FormItemType;
                    var PropertyEntityType = PropertyBinding.EntityType ?? FormEntityType;
                    var PropertyIDPropertyName = PropertyBinding.IDPropertyName ?? FormIDPropertyName;
                    var PropertyName = PropertyBinding.FieldType ?? Property.Name;

                    if (PropertyItemType == null || PropertyEntityType == null || PropertyIDPropertyName == null)
                    {
                        throw new InvalidOperationException("Binding is not configured properly.");
                    }

                    if (BoundItem.GetType() == PropertyItemType)
                    {
                        int? EntityIDFromForm = (int?)this.GetType().GetProperty(PropertyIDPropertyName).GetValue(this);
                        EntityBase Entity = null;
                        if (EntityIDFromForm.HasValue)
                        {
                            Entity = BoundItem.AllEntities.Where(e => e.EntityID == EntityIDFromForm).FirstOrDefault();
                        }
                        else
                        {
                            Entity = null; // EITODO BoundItem.EffectiveRecord.CreateEntity....
                        }

                        if (Entity == null || PropertyEntityType != Entity.GetType())
                        {
                            throw new InvalidOperationException("Cannot bind to entity.");
                        }

                        var EntityProperty = (IField)(PropertyEntityType.GetProperty(PropertyName).GetValue(Entity));

                        Property.SetValue(this, EntityProperty.Value);
                    }
                }
            }

            var AllFields = TypeOfForm.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var Field in AllFields)
            {
                var FieldBinding = Field.GetCustomAttribute<BindAttribute>();
                if (FieldBinding != null)
                {
                    var FieldItemType = FieldBinding.ItemType ?? FormItemType;
                    var FieldEntityType = FieldBinding.EntityType ?? FormEntityType;
                    var FieldIDPropertyName = FieldBinding.IDPropertyName ?? FormIDPropertyName;
                    var FieldName = FieldBinding.FieldType ?? Field.Name;

                    if (FieldItemType == null || FieldEntityType == null || FieldIDPropertyName == null)
                    {
                        throw new InvalidOperationException("Binding is not configured properly.");
                    }

                    if (BoundItem.GetType() == FieldItemType)
                    {
                        int? EntityIDFromForm = (int?)this.GetType().GetProperty(FieldIDPropertyName).GetValue(this);
                        EntityBase Entity = null;
                        if (EntityIDFromForm.HasValue)
                        {
                            Entity = BoundItem.AllEntities.Where(e => e.EntityID == EntityIDFromForm).FirstOrDefault();
                        }
                        else
                        {
                            Entity = null; // EITODO BoundItem.EffectiveRecord.CreateEntity....
                        }

                        if (Entity == null || FieldEntityType != Entity.GetType())
                        {
                            throw new InvalidOperationException("Cannot bind to entity.");
                        }

                        var EntityProperty = (IField)(FieldEntityType.GetProperty(FieldName).GetValue(Entity));

                        Field.SetValue(this, EntityProperty.Value);
                    }
                }
            }
        }

        public void BindTo(EntityBase Entity)
        {
            this.BoundEntity = Entity;
        }
    }
}
