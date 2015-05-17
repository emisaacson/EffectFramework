using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EffectFramework.Core.Models;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Models.Fields;

namespace EffectFramework.Core.Forms
{
    public abstract class Form
    {
        protected Dictionary<Type, Item> BoundItems { get; set; }

        private Type FormItemType = null;
        private Type FormEntityType = null;
        private string FormIDPropertyName = null;
        private string EffectiveDateFieldName = null;
        private string EndEffectiveDateFieldName = null;
        private enum Direction { Push, Pull };

        public Form()
        {
            ParseFormAttributes();
        }

        public void BindTo(params Item[] Items)
        {
            BoundItems = new Dictionary<Type, Item>();

            foreach (var Item in Items)
            {
                if (BoundItems.ContainsKey(Item.GetType()))
                {
                    throw new InvalidOperationException("Cannot bind to two entities of the same type.");
                }

                BoundItems[Item.GetType()] = Item;
            }
        }

        private void TransferValues(Direction Direction)
        {
            if (this.BoundItems == null)
            {
                throw new InvalidOperationException("Must bind the form to an item first.");
            }

            Dictionary<EntityType, EntityBase> EntityCache = new Dictionary<EntityType, EntityBase>();

            DateTime Now = DateTime.Now;

            var TypeOfForm = this.GetType();

            var AllProperties = TypeOfForm.GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(p => p.Name == EffectiveDateFieldName ? 0 : 1);
            var AllFields = TypeOfForm.GetFields(BindingFlags.Public | BindingFlags.Instance).OrderBy(f => f.Name == EffectiveDateFieldName ? 0 : 1);

            TransferValuesFromMembers(AllProperties, Direction, Now, ref EntityCache);
            TransferValuesFromMembers(AllFields, Direction, Now, ref EntityCache);
        }
        
        public void PopulateForm()
        {
            TransferValues(Direction.Pull);
        }

        public void PushValuesToModel()
        {
            TransferValues(Direction.Push);
        }

        private void ParseFormAttributes()
        {

            // Get form global bindings
            Type TypeOfForm = this.GetType();
            var FormBinding = TypeOfForm.GetCustomAttribute<BindAttribute>();
            var EffectiveDateBinding = TypeOfForm.GetCustomAttribute<EffectiveDateAttribute>();
            var EndEffectiveDateBinding = TypeOfForm.GetCustomAttribute<EndEffectiveDateAttribute>();

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
                    throw new InvalidOperationException("Must specify a field name for model level effective date binding.");
                }

                EffectiveDateFieldName = EffectiveDateBinding.FieldName;
            }

            if (EndEffectiveDateBinding != null)
            {
                if (EndEffectiveDateBinding.FieldName == null)
                {
                    throw new InvalidOperationException("Must specify a field name for model level end effective date binding.");
                }

                EndEffectiveDateFieldName = EndEffectiveDateBinding.FieldName;
            }

            // Get Property and field bindings
            var AllProperties = TypeOfForm.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            ParseAttributesFromMemberInfo(AllProperties, ref EffectiveDateBinding, ref EndEffectiveDateBinding);

            var AllFields = TypeOfForm.GetFields(BindingFlags.Public | BindingFlags.Instance);

            ParseAttributesFromMemberInfo(AllFields, ref EffectiveDateBinding, ref EndEffectiveDateBinding);

        }

        private void ParseAttributesFromMemberInfo(IEnumerable<MemberInfo> AllMembers, ref EffectiveDateAttribute EffectiveDateBinding, ref EndEffectiveDateAttribute EndEffectiveDateBinding)
        {
            foreach (var Member in AllMembers)
            {
                var MemberLevelEffectiveDateAttribute = Member.GetCustomAttribute<EffectiveDateAttribute>();
                var MemberLevelEndEffectiveDateAttribute = Member.GetCustomAttribute<EndEffectiveDateAttribute>();

                if (MemberLevelEffectiveDateAttribute != null && MemberLevelEffectiveDateAttribute.FieldName == null && EffectiveDateBinding != null)
                {
                    throw new InvalidOperationException("Cannot specify more than one Effective Date attribute on a model.");
                }

                if (MemberLevelEndEffectiveDateAttribute != null && MemberLevelEndEffectiveDateAttribute.FieldName == null && EndEffectiveDateBinding != null)
                {
                    throw new InvalidOperationException("Cannot specify more than one End Effective Date attribute on a model.");
                }

                if (MemberLevelEffectiveDateAttribute != null && MemberLevelEffectiveDateAttribute.FieldName == null)
                {
                    EffectiveDateBinding = MemberLevelEffectiveDateAttribute;
                    EffectiveDateFieldName = Member.Name;
                }

                if (MemberLevelEndEffectiveDateAttribute != null && MemberLevelEndEffectiveDateAttribute.FieldName == null)
                {
                    EndEffectiveDateBinding = MemberLevelEndEffectiveDateAttribute;
                    EndEffectiveDateFieldName = Member.Name;
                }

            }
        }

        private void TransferValuesFromMembers(IEnumerable<MemberInfo> AllMembers, Direction Direction, DateTime Now, ref Dictionary<EntityType, EntityBase> EntityCache)
        {
            foreach (var Member in AllMembers)
            {
                var PropertyBinding = Member.GetCustomAttribute<BindAttribute>();
                var PropertyLevelEffectiveDateAttribute = Member.GetCustomAttribute<EffectiveDateAttribute>();
                var PropertyLevelEndEffectiveDateAttribute = Member.GetCustomAttribute<EndEffectiveDateAttribute>();

                if (PropertyBinding != null)
                {
                    var PropertyItemType = PropertyBinding.ItemType ?? FormItemType;
                    var PropertyEntityType = PropertyBinding.EntityType ?? FormEntityType;
                    var PropertyIDPropertyName = PropertyBinding.IDPropertyName ?? FormIDPropertyName;
                    string PropertyEffectiveDateFieldName = EffectiveDateFieldName;
                    string PropertyEndEffectiveDateFieldName = EndEffectiveDateFieldName;
                    var PropertyName = PropertyBinding.FieldType ?? Member.Name;
                    Item BoundItem = BoundItems[PropertyItemType];

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
                        if (EffectiveDate == default(DateTime))
                        {
                            EffectiveDate = BoundItem.EffectiveDate;
                        }
                    }
                    if (PropertyEndEffectiveDateFieldName != null)
                    {
                        EndEffectiveDate = (DateTime?)this.GetType().GetProperty(PropertyEndEffectiveDateFieldName).GetValue(this);
                    }

                    if (BoundItem.GetType() == PropertyItemType)
                    {
                        int? EntityIDFromForm = (int?)this.GetType().GetProperty(PropertyIDPropertyName).GetValue(this);
                        EntityBase Entity = null;

                        EntityCollection EffectiveRecord = BoundItem.GetEntityCollectionForDate(EffectiveDate);
                        if (EntityIDFromForm.HasValue)
                        {
                            if (EntityCache.Values.Any(e => e.EntityID.Value == EntityIDFromForm.Value))
                            {
                                Entity = EntityCache.Values.First(e => e.EntityID.Value == EntityIDFromForm.Value);
                            }
                            else
                            {
                                if (PropertyEffectiveDateFieldName == null || (PropertyEffectiveDateFieldName != null && (DateTime)this.GetType().GetProperty(PropertyEffectiveDateFieldName).GetValue(this) == default(DateTime)))
                                {
                                    Entity = BoundItem.AllEntities.Where(e => e.EntityID == EntityIDFromForm).FirstOrDefault();
                                }
                                else
                                {
                                    Entity = EffectiveRecord.AllEntities.Where(e => e.EntityID == EntityIDFromForm).FirstOrDefault();
                                }
                                EntityCache[Entity.Type] = Entity;
                                if (Direction == Direction.Pull)
                                {
                                    this.GetType().GetProperty(PropertyIDPropertyName).SetValue(this, Entity.EntityID);
                                }
                            }

                        }
                        else
                        {
                            EntityBase Instance = (EntityBase)Activator.CreateInstance(PropertyEntityType);
                            if (EntityCache.ContainsKey(Instance.Type))
                            {
                                Entity = EntityCache[Instance.Type];
                            }
                            else
                            {
                                if (Direction == Direction.Pull)
                                {
                                    Entity = EffectiveRecord.GetOrCreateEntityButDontSave(Instance.Type);
                                }
                                else if (Direction == Direction.Push)
                                {
                                    Entity = EffectiveRecord.CreateEntityAndEndDateAllPrevious(Instance.Type);
                                }
                                EntityCache[Instance.Type] = Entity;
                                if (Direction == Direction.Pull)
                                {
                                    this.GetType().GetProperty(PropertyIDPropertyName).SetValue(this, Entity.EntityID);
                                }
                            }
                        }

                        if (Entity == null || PropertyEntityType != Entity.GetType())
                        {
                            throw new InvalidOperationException("Cannot bind to entity.");
                        }

                        if (PropertyName == "EffectiveDate") // Special case for this special field
                        {

                            if (Direction == Direction.Push)
                            {
                                Entity.EffectiveDate = (DateTime)GetValueFrom(Member);
                            }
                            else if (Direction == Direction.Pull)
                            {
                                SetValueOn(Member, Entity.EffectiveDate);
                            }
                        }
                        else if (PropertyName == "EndEffectiveDate") // Special case for this special field
                        {

                            if (Direction == Direction.Push)
                            {
                                Entity.EndEffectiveDate = (DateTime?)GetValueFrom(Member);
                            }
                            else if (Direction == Direction.Pull)
                            {
                                SetValueOn(Member, Entity.EndEffectiveDate);
                            }
                        }
                        else
                        {
                            IField EntityProperty;
                            try
                            {
                                EntityProperty = (IField)(PropertyEntityType.GetProperty(PropertyName).GetValue(Entity));
                            }
                            catch (NullReferenceException)
                            {
                                throw new InvalidOperationException("Binding is not configured properly.");
                            }

                            if (Direction == Direction.Push)
                            {
                                EntityProperty.Value = GetValueFrom(Member);
                            }
                            else if (Direction == Direction.Pull)
                            {
                                SetValueOn(Member, EntityProperty.Value);
                            }
                        }
                    }
                }
            }
        }

        private void SetValueOn(MemberInfo Member, object Value)
        {
            if (Member is PropertyInfo)
            {
                ((PropertyInfo)Member).SetValue(this, Value);
            }
            else if (Member is FieldInfo)
            {
                ((FieldInfo)Member).SetValue(this, Value);
            }
            else
            {
                throw new ArgumentException("Can only set value on a Property or Field.");
            }
        }

        private object GetValueFrom(MemberInfo Member)
        {
            if (Member is PropertyInfo)
            {
                return ((PropertyInfo)Member).GetValue(this);
            }
            else if (Member is FieldInfo)
            {
                return ((FieldInfo)Member).GetValue(this);
            }

            throw new ArgumentException("Can only set value on a Property or Field.");
        }
    }
}
