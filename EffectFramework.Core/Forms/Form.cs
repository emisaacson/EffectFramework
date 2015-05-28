using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EffectFramework.Core.Models;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Models.Fields;
using EffectFramework.Core.Services;
using Ninject;

namespace EffectFramework.Core.Forms
{
    /// <summary>
    /// A Form is used to bundle fields coming from some external source and map
    /// them to one or more EffectFramework entities. Attributes on the form class
    /// and properties are used to configure the bindings. Forms can be used to
    /// push or pull data from the Item models.
    /// </summary>
    public abstract class Form
    {
        protected Dictionary<Type, Item> BoundItems { get; set; }


        // The fields below are the result of running ParseFormAttributes,
        // which is run when the form is first initialized and stores information
        // about the Binding attributes on the form class.
        private Type FormItemType = null;
        private Type FormEntityType = null;
        private string FormIDMemberName = null;
        private string EffectiveDateMemberName = null;
        private string EndEffectiveDateMemberName = null;


        private enum Direction { Push, Pull };

        public Form()
        {
            ParseFormAttributes();
        }

        /// <summary>
        /// Adds an Item to the list of bound Items for this form. Multiple Items
        /// may be specified, but they must all be different ItemTypes.
        /// </summary>
        /// <param name="Items">A variable number of Items to bind to the form.</param>
        /// <exception cref="System.InvalidOperationException">Cannot bind to two entities of the same type.</exception>
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

        /// <summary>
        /// Pulls or pushes information between the current form and the
        /// bound objects, depending on the passed Direction flag.
        /// </summary>
        /// <param name="Direction">The direction.</param>
        /// <exception cref="System.InvalidOperationException">Must bind the form to an item first.</exception>
        private void TransferValues(Direction Direction)
        {
            if (this.BoundItems == null)
            {
                throw new InvalidOperationException("Must bind the form to an item first.");
            }

            Dictionary<EntityType, EntityBase> EntityCache = new Dictionary<EntityType, EntityBase>();

            DateTime Now = DateTime.Now.Date;

            var TypeOfForm = this.GetType();

            var AllProperties = TypeOfForm.GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(p => p.Name == EffectiveDateMemberName ? 0 : 1);
            var AllFields = TypeOfForm.GetFields(BindingFlags.Public | BindingFlags.Instance).OrderBy(f => f.Name == EffectiveDateMemberName ? 0 : 1);

            TransferValuesFromMembers(AllProperties, Direction, Now, ref EntityCache);
            TransferValuesFromMembers(AllFields, Direction, Now, ref EntityCache);
        }


        /// <summary>
        /// Take values from the bound Items and transfer them to the
        /// members of the Form object, according to the Form's Binding
        /// attributes.
        /// </summary>
        public void PopulateForm()
        {
            TransferValues(Direction.Pull);
        }

        /// <summary>
        /// Take values from the Form object and transfer them to the bound
        /// Items, according to the Form's Binding attributes.
        /// </summary>
        public void PushValuesToModel()
        {
            TransferValues(Direction.Push);
        }


        /// <summary>
        /// Parses the form attributes.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Must specify a field name for model level effective date binding.
        /// or
        /// Must specify a field name for model level end effective date binding.
        /// </exception>
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
                FormIDMemberName = FormBinding.IDPropertyName;
            }

            if (EffectiveDateBinding != null)
            {
                if (EffectiveDateBinding.FieldName == null)
                {
                    throw new InvalidOperationException("Must specify a field name for model level effective date binding.");
                }

                EffectiveDateMemberName = EffectiveDateBinding.FieldName;
            }

            if (EndEffectiveDateBinding != null)
            {
                if (EndEffectiveDateBinding.FieldName == null)
                {
                    throw new InvalidOperationException("Must specify a field name for model level end effective date binding.");
                }

                EndEffectiveDateMemberName = EndEffectiveDateBinding.FieldName;
            }

            // Get Property and field bindings
            var AllProperties = TypeOfForm.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var AllFields = TypeOfForm.GetFields(BindingFlags.Public | BindingFlags.Instance);

            ParseAttributesFromMemberInfo(AllProperties, ref EffectiveDateBinding, ref EndEffectiveDateBinding);
            ParseAttributesFromMemberInfo(AllFields, ref EffectiveDateBinding, ref EndEffectiveDateBinding);
        }

        /// <summary>
        /// This was factored out of the ParseFormAttributes method to avoid code duplication
        /// between performing these operations on both Fields and Properties. The ref
        /// parameters are local state from the containing ParseFormAttributes method. This
        /// method should not be used in any other context.
        /// </summary>
        /// <param name="AllMembers">All members.</param>
        /// <param name="EffectiveDateBinding">The effective date binding.</param>
        /// <param name="EndEffectiveDateBinding">The end effective date binding.</param>
        /// <exception cref="System.InvalidOperationException">
        /// Cannot specify more than one Effective Date attribute on a model.
        /// or
        /// Cannot specify more than one End Effective Date attribute on a model.
        /// </exception>
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
                    EffectiveDateMemberName = Member.Name;
                }

                if (MemberLevelEndEffectiveDateAttribute != null && MemberLevelEndEffectiveDateAttribute.FieldName == null)
                {
                    EndEffectiveDateBinding = MemberLevelEndEffectiveDateAttribute;
                    EndEffectiveDateMemberName = Member.Name;
                }

            }
        }

        /// <summary>
        /// Gets a new instance of the field bound to this member on the form.
        /// </summary>
        /// <param name="MemberName">Name of the form field or property.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The specified field does not exist.</exception>
        /// <exception cref="System.InvalidOperationException">Binding is not configured properly.</exception>
        public IField GetBoundField(string MemberName)
        {
            if (MemberName == null)
            {
                throw new ArgumentNullException();
            }
            MemberInfo Member = this.GetType().GetProperty(MemberName);
            if (Member == null)
            {
                Member = this.GetType().GetField(MemberName);
            }
            if (Member == null)
            {
                throw new ArgumentOutOfRangeException("The specified field does not exist.");
            }
            var MemberBinding = Member.GetCustomAttribute<BindAttribute>();
            var MemberLevelEffectiveDateAttribute = Member.GetCustomAttribute<EffectiveDateAttribute>();
            var MemberLevelEndEffectiveDateAttribute = Member.GetCustomAttribute<EndEffectiveDateAttribute>();

            if (MemberBinding != null)
            {
                var MemberItemType = MemberBinding.ItemType ?? FormItemType;
                var MemberEntityType = MemberBinding.EntityType ?? FormEntityType;
                var MemberIDMemberName = MemberBinding.IDPropertyName ?? FormIDMemberName;
                string MemberEffectiveDateFieldName = EffectiveDateMemberName;
                string MemberEndEffectiveDateFieldName = EndEffectiveDateMemberName;
                var _MemberName = MemberBinding.FieldType ?? Member.Name;

                if (MemberItemType == null || MemberEntityType == null || MemberIDMemberName == null)
                {
                    throw new InvalidOperationException("Binding is not configured properly.");
                }

                if (MemberLevelEffectiveDateAttribute != null && MemberLevelEffectiveDateAttribute.FieldName != null)
                {
                    MemberEffectiveDateFieldName = MemberLevelEffectiveDateAttribute.FieldName;
                }
                if (MemberLevelEndEffectiveDateAttribute != null && MemberLevelEndEffectiveDateAttribute.FieldName != null)
                {
                    MemberEndEffectiveDateFieldName = MemberLevelEndEffectiveDateAttribute.FieldName;
                }

                MemberInfo EntityMemberInfo = MemberEntityType.GetProperty(_MemberName);
                if (EntityMemberInfo == null)
                {
                    EntityMemberInfo = MemberEntityType.GetField(_MemberName);
                }
                if (EntityMemberInfo == null)
                {
                    throw new InvalidOperationException("Entity schema is not correct.");
                }

                EntityBase Entity = null;
                object EntityMember = null;
                using (IKernel Kernel = new StandardKernel(new Configure()))
                {
                    Entity = (EntityBase)Kernel.Get(MemberEntityType);
                }
                if (EntityMemberInfo.MemberType == MemberTypes.Property)
                {
                    EntityMember = ((PropertyInfo)EntityMemberInfo).GetValue(Entity);
                }
                else if (EntityMemberInfo.MemberType == MemberTypes.Field)
                {
                    EntityMember = ((FieldInfo)EntityMemberInfo).GetValue(Entity);
                }
                if (EntityMember is IField)
                {
                    return (IField)EntityMember;
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// This was factored out of the TransferValues method to avoid code duplication
        /// between performing these operations on both Fields and Properties. The ref
        /// parameters are local state from the containing TransferValues method. This
        /// method should not be used in any other context.
        /// The logic below is complicated but here is the general idea:
        /// * Only members with [Bind] on the for will be included in any push or pull operation
        /// * Global defaults for the item type, entity type, and EntityID / EffectiveDate / EndEffectiveDate
        /// are specified on the class
        /// * If EffectiveDate or EndEffectiveDate attributes are used on a member, it is also a global
        /// default
        /// * Individual members can be bound differently if specified on the particular member's Bind
        /// attribute.
        /// * If you an EntityID is present in the configured EntityID binding on the form, it is used,
        /// otherwise a new entity is found or created. Found in the case of Pull, created if find
        /// fails and in all cases with push.
        /// * If there is any unresolvable binding or discrepencies in the Binding an InvalidOperationException
        /// is thrown.
        /// * Forms can have fields spanning multiple entities across multiple items, but only one Item per
        /// ItemType and one entity per EntityType. Use multiple form objects for multiple Items or Entities
        /// of the same types.
        /// </summary>
        /// <param name="AllMembers">Reflected properties or fields of the form.</param>
        /// <param name="Direction">The direction.</param>
        /// <param name="Now">The time right before this function is called.</param>
        /// <param name="EntityCache">Entity cache object.</param>
        /// <exception cref="System.InvalidOperationException">
        /// Binding is not configured properly.
        /// or
        /// Cannot bind to entity.
        /// or
        /// Binding is not configured properly.
        /// </exception>
        private void TransferValuesFromMembers(IEnumerable<MemberInfo> AllMembers, Direction Direction, DateTime Now, ref Dictionary<EntityType, EntityBase> EntityCache)
        {
            foreach (var Member in AllMembers)
            {
                var MemberBinding = Member.GetCustomAttribute<BindAttribute>();
                var MemberLevelEffectiveDateAttribute = Member.GetCustomAttribute<EffectiveDateAttribute>();
                var MemberLevelEndEffectiveDateAttribute = Member.GetCustomAttribute<EndEffectiveDateAttribute>();

                if (MemberBinding != null)
                {
                    var MemberItemType = MemberBinding.ItemType ?? FormItemType;
                    var MemberEntityType = MemberBinding.EntityType ?? FormEntityType;
                    var MemberIDMemberName = MemberBinding.IDPropertyName ?? FormIDMemberName;
                    string MemberEffectiveDateFieldName = EffectiveDateMemberName;
                    string MemberEndEffectiveDateFieldName = EndEffectiveDateMemberName;
                    var MemberName = MemberBinding.FieldType ?? Member.Name;
                    Item BoundItem = BoundItems[MemberItemType];

                    if (MemberItemType == null || MemberEntityType == null || MemberIDMemberName == null)
                    {
                        throw new InvalidOperationException("Binding is not configured properly.");
                    }

                    if (MemberLevelEffectiveDateAttribute != null && MemberLevelEffectiveDateAttribute.FieldName != null)
                    {
                        MemberEffectiveDateFieldName = MemberLevelEffectiveDateAttribute.FieldName;
                    }
                    if (MemberLevelEndEffectiveDateAttribute != null && MemberLevelEndEffectiveDateAttribute.FieldName != null)
                    {
                        MemberEndEffectiveDateFieldName = MemberLevelEndEffectiveDateAttribute.FieldName;
                    }

                    DateTime EffectiveDate = Now;
                    DateTime? EndEffectiveDate = null;
                    if (MemberEffectiveDateFieldName != null)
                    {
                        EffectiveDate = (DateTime)this.GetType().GetProperty(MemberEffectiveDateFieldName).GetValue(this);
                        if (EffectiveDate == default(DateTime))
                        {
                            EffectiveDate = BoundItem.EffectiveDate;
                        }
                    }
                    if (MemberEndEffectiveDateFieldName != null)
                    {
                        EndEffectiveDate = (DateTime?)this.GetType().GetProperty(MemberEndEffectiveDateFieldName).GetValue(this);
                    }

                    if (BoundItem.GetType() == MemberItemType)
                    {
                        int? EntityIDFromForm = (int?)this.GetType().GetProperty(MemberIDMemberName).GetValue(this);
                        if (EntityIDFromForm.HasValue && EntityIDFromForm.Value == default(int))
                        {
                            EntityIDFromForm = null;
                        }
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
                                if (MemberEffectiveDateFieldName == null || (MemberEffectiveDateFieldName != null && (DateTime)this.GetType().GetProperty(MemberEffectiveDateFieldName).GetValue(this) == default(DateTime)))
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
                                    this.GetType().GetProperty(MemberIDMemberName).SetValue(this, Entity.EntityID);
                                }
                            }

                        }
                        else
                        {
                            EntityBase Instance = (EntityBase)Activator.CreateInstance(MemberEntityType);
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
                                    Entity = EffectiveRecord.CreateEntityAndMaybeAdjustNeighbors(Instance.Type);
                                }
                                EntityCache[Instance.Type] = Entity;
                                if (Direction == Direction.Pull)
                                {
                                    this.GetType().GetProperty(MemberIDMemberName).SetValue(this, Entity.EntityID);
                                }
                            }
                        }

                        if (Entity == null || MemberEntityType != Entity.GetType())
                        {
                            throw new InvalidOperationException("Cannot bind to entity.");
                        }

                        if (MemberName == "EffectiveDate") // Special case for this special field
                        {

                            if (Direction == Direction.Push)
                            {
                                Entity.EffectiveDate = EffectiveDate;
                            }
                            else if (Direction == Direction.Pull)
                            {
                                SetValueOn(Member, Entity.EffectiveDate);
                            }
                        }
                        else if (MemberName == "EndEffectiveDate") // Special case for this special field
                        {

                            if (Direction == Direction.Push)
                            {
                                Entity.EndEffectiveDate = EndEffectiveDate;
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
                                EntityProperty = (IField)(MemberEntityType.GetProperty(MemberName).GetValue(Entity));
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

        /// <summary>
        /// Sets the value on on this form from the provided Field or Property
        /// Info object. There's no common interface between FieldInfo and PropertyInfo
        /// that includes the SetValue method, which makes this method necessary.
        /// </summary>
        /// <param name="Member">A PropertyInfo or FieldInfo object.</param>
        /// <param name="Value">The value to set.</param>
        /// <exception cref="System.ArgumentException">Can only set value on a Property or Field.</exception>
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

        /// <summary>
        /// Gets the value on from this form from the provided Field or Property
        /// Info object. There's no common interface between FieldInfo and PropertyInfo
        /// that includes the GetValue method, which makes this method necessary.
        /// </summary>
        /// <param name="Member">A PropertyInfo or FieldInfo object.</param>
        /// <returns>
        /// The value from the current Form object.
        /// </returns>
        /// <exception cref="System.ArgumentException">Can only set value on a Property or Field.</exception>
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
