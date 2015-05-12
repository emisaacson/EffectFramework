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

        // EITODO: refactor for readability and code duplication
        public void Populate()
        {
            if (this.BoundItem == null)
            {
                throw new InvalidOperationException("Must bind the form to an item first.");
            }

            DateTime Now = DateTime.Now;

            // Get form global bindings
            Type TypeOfForm = this.GetType();
            var FormBinding = TypeOfForm.GetCustomAttribute<BindAttribute>();
            var EffectiveDateBinding = TypeOfForm.GetCustomAttribute<EffectiveDateAttribute>();
            var EndEffectiveDateBinding = TypeOfForm.GetCustomAttribute<EndEffectiveDateAttribute>();

            Type FormItemType = null;
            Type FormEntityType = null;
            string FormIDPropertyName = null;
            string EffectiveDateFieldName = null;
            string EndEffectiveDateFieldName = null;

            if (FormBinding != null)
            {
                FormItemType = FormBinding.ItemType;
                FormEntityType = FormBinding.EntityType;
                FormIDPropertyName = FormBinding.IDPropertyName;
            }

            if (EffectiveDateBinding != null)
            {
                if (EffectiveDateBinding.FieldName == null)
                {
                    throw new InvalidOperationException("Most specify a field name for model level effective date binding.");
                }

                EffectiveDateFieldName = EffectiveDateBinding.FieldName;
            }

            if (EndEffectiveDateBinding != null)
            {
                if (EndEffectiveDateBinding.FieldName == null)
                {
                    throw new InvalidOperationException("Most specify a field name for model level end effective date binding.");
                }

                EndEffectiveDateFieldName = EndEffectiveDateBinding.FieldName;
            }

            // Get Property and field bindings
            var AllProperties = TypeOfForm.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var Property in AllProperties)
            {
                var PropertyLevelEffectiveDateAttribute = Property.GetCustomAttribute<EffectiveDateAttribute>();
                var PropertyLevelEndEffectiveDateAttribute = Property.GetCustomAttribute<EndEffectiveDateAttribute>();

                if (PropertyLevelEffectiveDateAttribute != null && PropertyLevelEffectiveDateAttribute.FieldName == null && EffectiveDateBinding != null)
                {
                    throw new InvalidOperationException("Cannot specify more than one Effective Date attribute on a model.");
                }

                if (PropertyLevelEndEffectiveDateAttribute != null && PropertyLevelEndEffectiveDateAttribute.FieldName == null && EndEffectiveDateBinding != null)
                {
                    throw new InvalidOperationException("Cannot specify more than one End Effective Date attribute on a model.");
                }

                if (PropertyLevelEffectiveDateAttribute != null && PropertyLevelEffectiveDateAttribute.FieldName == null)
                {
                    EffectiveDateBinding = PropertyLevelEffectiveDateAttribute;
                    EffectiveDateFieldName = Property.Name;
                }

                if (PropertyLevelEndEffectiveDateAttribute != null && PropertyLevelEndEffectiveDateAttribute.FieldName == null)
                {
                    EndEffectiveDateBinding = PropertyLevelEndEffectiveDateAttribute;
                    EndEffectiveDateFieldName = Property.Name;
                }

            }

            var AllFields = TypeOfForm.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var Field in AllFields)
            {
                var FieldLevelEffectiveDateAttribute = Field.GetCustomAttribute<EffectiveDateAttribute>();
                var FieldLevelEndEffectiveDateAttribute = Field.GetCustomAttribute<EndEffectiveDateAttribute>();

                if (FieldLevelEffectiveDateAttribute != null && FieldLevelEffectiveDateAttribute.FieldName == null && EffectiveDateBinding != null)
                {
                    throw new InvalidOperationException("Cannot specify more than one Effective Date attribute on a model.");
                }

                if (FieldLevelEndEffectiveDateAttribute != null && FieldLevelEndEffectiveDateAttribute.FieldName == null && EndEffectiveDateBinding != null)
                {
                    throw new InvalidOperationException("Cannot specify more than one End Effective Date attribute on a model.");
                }

                if (FieldLevelEffectiveDateAttribute != null && FieldLevelEffectiveDateAttribute.FieldName == null)
                {
                    EffectiveDateBinding = FieldLevelEffectiveDateAttribute;
                    EffectiveDateFieldName = Field.Name;
                }

                if (FieldLevelEndEffectiveDateAttribute != null && FieldLevelEndEffectiveDateAttribute.FieldName == null)
                {
                    EndEffectiveDateBinding = FieldLevelEndEffectiveDateAttribute;
                    EndEffectiveDateFieldName = Field.Name;
                }

            }

            foreach (var Property in AllProperties)
            {
                var PropertyBinding = Property.GetCustomAttribute<BindAttribute>();
                var PropertyLevelEffectiveDateAttribute = Property.GetCustomAttribute<EffectiveDateAttribute>();
                var PropertyLevelEndEffectiveDateAttribute = Property.GetCustomAttribute<EndEffectiveDateAttribute>();

                if (PropertyBinding != null)
                {
                    var PropertyItemType = PropertyBinding.ItemType ?? FormItemType;
                    var PropertyEntityType = PropertyBinding.EntityType ?? FormEntityType;
                    var PropertyIDPropertyName = PropertyBinding.IDPropertyName ?? FormIDPropertyName;
                    string PropertyEffectiveDateFieldName = EffectiveDateFieldName;
                    string PropertyEndEffectiveDateFieldName = EndEffectiveDateFieldName;
                    var PropertyName = PropertyBinding.FieldType ?? Property.Name;

                    if (PropertyItemType == null || PropertyEntityType == null || PropertyIDPropertyName == null)
                    {
                        throw new InvalidOperationException("Binding is not configured properly.");
                    }

                    if (PropertyLevelEffectiveDateAttribute != null && PropertyLevelEffectiveDateAttribute.FieldName != null)
                    {
                        PropertyEffectiveDateFieldName = PropertyLevelEffectiveDateAttribute.FieldName;
                    }
                    if (PropertyLevelEndEffectiveDateAttribute != null && PropertyLevelEndEffectiveDateAttribute.FieldName != null)
                    {
                        PropertyEndEffectiveDateFieldName = PropertyLevelEndEffectiveDateAttribute.FieldName;
                    }

                    DateTime EffectiveDate = Now;
                    DateTime? EndEffectiveDate = null;
                    if (PropertyEffectiveDateFieldName != null)
                    {
                        EffectiveDate = (DateTime)this.GetType().GetProperty(PropertyEffectiveDateFieldName).GetValue(this);
                    }
                    if (PropertyEndEffectiveDateFieldName != null)
                    {
                        EndEffectiveDate = (DateTime?)this.GetType().GetProperty(PropertyEndEffectiveDateFieldName).GetValue(this);
                    }

                    if (BoundItem.GetType() == PropertyItemType)
                    {
                        int? EntityIDFromForm = (int?)this.GetType().GetProperty(PropertyIDPropertyName).GetValue(this);
                        EntityBase Entity = null;

                        BoundItem.EffectiveDate = EffectiveDate;
                        if (EntityIDFromForm.HasValue)
                        {
                            if (PropertyEffectiveDateFieldName == null)
                            {
                                Entity = BoundItem.AllEntities.Where(e => e.EntityID == EntityIDFromForm).FirstOrDefault();
                            }
                            else
                            {
                                Entity = BoundItem.EffectiveRecord.AllEntities.Where(e => e.EntityID == EntityIDFromForm).FirstOrDefault();
                            }

                        }
                        else
                        {
                            EntityBase Instance = (EntityBase)Activator.CreateInstance(PropertyEntityType);
                            Entity = BoundItem.EffectiveRecord.GetOrCreateEntityButDontSave(Instance.Type);
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

            foreach (var Field in AllFields)
            {
                var FieldBinding = Field.GetCustomAttribute<BindAttribute>();
                var FieldLevelEffectiveDateAttribute = Field.GetCustomAttribute<EffectiveDateAttribute>();
                var FieldLevelEndEffectiveDateAttribute = Field.GetCustomAttribute<EndEffectiveDateAttribute>();

                if (FieldBinding != null)
                {
                    var FieldItemType = FieldBinding.ItemType ?? FormItemType;
                    var FieldEntityType = FieldBinding.EntityType ?? FormEntityType;
                    var FieldIDPropertyName = FieldBinding.IDPropertyName ?? FormIDPropertyName;
                    string FieldEffectiveDateFieldName = EffectiveDateFieldName;
                    string FieldEndEffectiveDateFieldName = EndEffectiveDateFieldName;
                    var FieldName = FieldBinding.FieldType ?? Field.Name;

                    if (FieldItemType == null || FieldEntityType == null || FieldIDPropertyName == null)
                    {
                        throw new InvalidOperationException("Binding is not configured properly.");
                    }

                    if (FieldLevelEffectiveDateAttribute != null && FieldLevelEffectiveDateAttribute.FieldName != null)
                    {
                        FieldEffectiveDateFieldName = FieldLevelEffectiveDateAttribute.FieldName;
                    }
                    if (FieldLevelEndEffectiveDateAttribute != null && FieldLevelEndEffectiveDateAttribute.FieldName != null)
                    {
                        FieldEndEffectiveDateFieldName = FieldLevelEndEffectiveDateAttribute.FieldName;
                    }

                    DateTime EffectiveDate = Now;
                    DateTime? EndEffectiveDate = null;
                    if (FieldEffectiveDateFieldName != null)
                    {
                        EffectiveDate = (DateTime)this.GetType().GetProperty(FieldEffectiveDateFieldName).GetValue(this);
                    }
                    if (FieldEndEffectiveDateFieldName != null)
                    {
                        EndEffectiveDate = (DateTime?)this.GetType().GetProperty(FieldEndEffectiveDateFieldName).GetValue(this);
                    }

                    if (BoundItem.GetType() == FieldItemType)
                    {
                        int? EntityIDFromForm = (int?)this.GetType().GetProperty(FieldIDPropertyName).GetValue(this);
                        EntityBase Entity = null;

                        BoundItem.EffectiveDate = EffectiveDate;
                        if (EntityIDFromForm.HasValue)
                        {
                            if (FieldEffectiveDateFieldName == null)
                            {
                                Entity = BoundItem.AllEntities.Where(e => e.EntityID == EntityIDFromForm).FirstOrDefault();
                            }
                            else
                            {
                                Entity = BoundItem.EffectiveRecord.AllEntities.Where(e => e.EntityID == EntityIDFromForm).FirstOrDefault();
                            }
                        }
                        else
                        {
                            EntityBase Instance = (EntityBase)Activator.CreateInstance(FieldEntityType);
                            Entity = BoundItem.EffectiveRecord.GetOrCreateEntityButDontSave(Instance.Type);
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

        public void Persist()
        {
            if (this.BoundItem == null)
            {
                throw new InvalidOperationException("Must bind the form to an item first.");
            }


        }

        public void BindTo(EntityBase Entity)
        {
            this.BoundEntity = Entity;
        }
    }
}
