using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EffectFramework.Core.Models;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Models.Fields;

namespace EffectFramework.Core.Forms
{
    /// <summary>
    /// A Form is used to bundle fields coming from some external source and map
    /// them to one or more EffectFramework entities.
    /// </summary>
    /// <remarks>Attributes on the form class
    /// and properties are used to configure the bindings. Forms can be used to
    /// push or pull data from the Item models.
    /// </remarks>
    public abstract class Form
    {
        protected Logger _Log;
        protected Logger Log
        {
            get
            {
                if (_Log == null)
                {
                    _Log = new Logger(GetType().Name);
                }
                return _Log;
            }
        }

        /// <summary>
        /// This is a hash map of field name to entity ID.
        /// </summary>
        /// <remarks>This is to prevent having to
        /// send large amounts of data in case a binary field is not changing.
        /// 
        /// If set for a particular field, the value for the field will be
        /// done by pulling it from the supplied EntityID instead of from the
        /// value on the form.
        /// </remarks>
        protected Dictionary<string, int> FormMembersNotToChange = new Dictionary<string, int>();
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

        public virtual string Name
        {
            get
            {
                return GetType().Name;
            }
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

            // It's very important to send process the effective date fields first so the other fields can reference them
            // EITODO: BUG: If there are other effective date fields defined besides form defaults then this will fail.
            var AllProperties = TypeOfForm.GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(p => p.Name == EffectiveDateMemberName ? 0 : 1);
            var AllFields = TypeOfForm.GetFields(BindingFlags.Public | BindingFlags.Instance).OrderBy(f => f.Name == EffectiveDateMemberName ? 0 : 1);

            TransferValuesFromMembers(AllProperties, Direction, Now, ref EntityCache);
            TransferValuesFromMembers(AllFields, Direction, Now, ref EntityCache);

            if (Direction == Direction.Push)
            {
                foreach (var Entity in EntityCache.Values)
                {
                    Entity.Item.PerformUpdate(Entity);
                }
            }
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
                    Log.Fatal("Binding is not configured properly, cannot specify more than one Effective Date attribute on a model. Member Name: {0}", Member.Name);
                    throw new InvalidOperationException("Cannot specify more than one Effective Date attribute on a model.");
                }

                if (MemberLevelEndEffectiveDateAttribute != null && MemberLevelEndEffectiveDateAttribute.FieldName == null && EndEffectiveDateBinding != null)
                {
                    Log.Fatal("Cannot specify more than one End Effective Date attribute on a model. Member Name: {0}", Member.Name);
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
        /// Gets the item bound to this member on the form.
        /// </summary>
        /// <param name="MemberName">Name of the form field or property.</param>
        /// <returns>A new instance of the bound entity, or null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The specified field does not exist.</exception>
        /// <exception cref="System.InvalidOperationException">Binding is not configured properly.</exception>
        public Item GetBoundItem(string MemberName)
        {
            if (MemberName == null)
            {
                Log.Error("A null member name was passed to GetBoundItem.");
                throw new ArgumentNullException(nameof(MemberName));
            }
            MemberInfo Member = this.GetType().GetProperty(MemberName);
            if (Member == null)
            {
                Member = this.GetType().GetField(MemberName);
            }
            if (Member == null)
            {
                Log.Error("An invalid MemberName was passed to GetBoundItem. MemberName: {0}", MemberName);
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
                Item BoundItem = BoundItems != null && BoundItems.ContainsKey(MemberItemType) ? BoundItems[MemberItemType] : Item.CreateItem(MemberItemType);

                if (MemberItemType == null || MemberEntityType == null || MemberIDMemberName == null)
                {
                    Log.Error("Not able to get bound item due to improper binding.");
                    throw new InvalidOperationException("Binding is not configured properly.");
                }

                return BoundItem;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a new instance of the entity bound to this member on the form.
        /// </summary>
        /// <param name="MemberName">Name of the form field or property.</param>
        /// <returns>A new instance of the bound entity, or null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The specified field does not exist.</exception>
        /// <exception cref="System.InvalidOperationException">Binding is not configured properly.</exception>
        public EntityBase GetBoundEntity(string MemberName)
        {
            if (MemberName == null)
            {
                Log.Error("A null member name was passed to GetBoundEntity.");
                throw new ArgumentNullException(nameof(MemberName));
            }
            MemberInfo Member = this.GetType().GetProperty(MemberName);
            if (Member == null)
            {
                Member = this.GetType().GetField(MemberName);
            }
            if (Member == null)
            {
                Log.Error("An invalid MemberName was passed to GetBoundEntity. MemberName: {0}", MemberName);
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
                Item BoundItem = BoundItems != null && BoundItems.ContainsKey(MemberItemType) ? BoundItems[MemberItemType] : Item.CreateItem(MemberItemType);

                if (MemberItemType == null || MemberEntityType == null || MemberIDMemberName == null)
                {
                    Log.Error("Not able to get bound entity due to improper binding.");
                    throw new InvalidOperationException("Binding is not configured properly.");
                }

                EntityBase Entity = EntityBase.GetEntityBySystemType(MemberEntityType, BoundItem);

                return Entity;
            }
            else
            {
                return null;
            }
        }

        public string GetFormFieldNameIfBound(FieldBase Field)
        {
            if (Field == null)
            {
                throw new ArgumentNullException(nameof(Field));
            }

            var AllFields = GetBoundFields();

            foreach (var _Field in AllFields)
            {
                var BoundField = (FieldBase)GetBoundField(_Field.Name);
                if (BoundField.Entity.Item.Type == Field.Entity.Item.Type &&
                    BoundField.Entity.Type == Field.Entity.Type &&
                    BoundField.Type == Field.Type)
                {
                    return _Field.Name;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a new instance of the field bound to this member on the form.
        /// </summary>
        /// <param name="MemberName">Name of the form field or property.</param>
        /// <returns>A new instance of the bound field, or null.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The specified field does not exist.</exception>
        /// <exception cref="System.InvalidOperationException">Binding is not configured properly.</exception>
        public IField GetBoundField(string MemberName)
        {
            if (MemberName == null)
            {
                throw new ArgumentNullException(nameof(MemberName));
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
                Item BoundItem = BoundItems != null && BoundItems.ContainsKey(MemberItemType) ? BoundItems[MemberItemType] : Item.CreateItem(MemberItemType);

                if (MemberItemType == null || MemberEntityType == null || MemberIDMemberName == null)
                {
                    Log.Error("Not able to get bound field due to improper binding.");
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
                    Log.Error("Could not get a binding for MemberName: {0}", _MemberName);
                    throw new InvalidOperationException("Entity schema is not correct.");
                }

                EntityBase Entity = EntityBase.GetEntityBySystemType(MemberEntityType, BoundItem);
                object EntityMember = null;

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
        /// between performing these operations on both Fields and Properties.
        /// </summary>
        /// <remarks>The ref
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
        /// </remarks>
        /// <param name="AllMembers">Reflected properties or fields of the form. They should be ordered so the effective date fields come FIRST.</param>
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
            Log.Trace("Entering TransferValuesFromMembers");

            foreach (var Member in AllMembers)
            {
                // Get any attribute set on the individual members
                var MemberBinding = Member.GetCustomAttribute<BindAttribute>();
                var MemberLevelEffectiveDateAttribute = Member.GetCustomAttribute<EffectiveDateAttribute>();
                var MemberLevelEndEffectiveDateAttribute = Member.GetCustomAttribute<EndEffectiveDateAttribute>();

                if (MemberBinding != null)
                {
                    // Set the ItemType, EntityType, EntityIDMemberName, Effective Date Field Name,
                    // Effective Date End Field Name, Member Name. These are first taken from any
                    // binding set directly on the member, but uses the global form defaults if there
                    // isn't anything set on the member directly.
                    var MemberItemType = MemberBinding.ItemType ?? FormItemType;
                    var MemberEntityType = MemberBinding.EntityType ?? FormEntityType;
                    var MemberIDMemberName = MemberBinding.IDPropertyName ?? FormIDMemberName;
                    string MemberEffectiveDateFieldName = EffectiveDateMemberName;
                    string MemberEndEffectiveDateFieldName = EndEffectiveDateMemberName;
                    var MemberName = MemberBinding.FieldType ?? Member.Name;
                    Item BoundItem = BoundItems[MemberItemType];

                    if (MemberItemType == null || MemberEntityType == null || MemberIDMemberName == null)
                    {
                        Log.Error("Not able to transfer members due to improper binding.");
                        throw new InvalidOperationException("Binding is not configured properly.");
                    }

                    // Effective date and end effective date field name can override the form defaults if set
                    // directly on the member
                    if (MemberLevelEffectiveDateAttribute != null && MemberLevelEffectiveDateAttribute.FieldName != null)
                    {
                        MemberEffectiveDateFieldName = MemberLevelEffectiveDateAttribute.FieldName;
                    }
                    if (MemberLevelEndEffectiveDateAttribute != null && MemberLevelEndEffectiveDateAttribute.FieldName != null)
                    {
                        MemberEndEffectiveDateFieldName = MemberLevelEndEffectiveDateAttribute.FieldName;
                    }

                    // If no other information is given, we default the effective date to now.
                    DateTime EffectiveDate = Now;
                    DateTime? EndEffectiveDate = null;

                    // If an effective date field name exists, we get its value from the form.
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

                    // EITODO: won't this always be true?
                    if (BoundItem.GetType() == MemberItemType)
                    {
                        // Get the entity ID. If 0 is found, assume null.
                        int? EntityIDFromForm = (int?)this.GetType().GetProperty(MemberIDMemberName).GetValue(this);
                        if (EntityIDFromForm.HasValue && EntityIDFromForm.Value == default(int))
                        {
                            EntityIDFromForm = null;
                        }
                        EntityBase Entity = null;

                        // Set the effective record on the Item
                        EntityCollection EffectiveRecord = BoundItem.GetEntityCollectionForDate(EffectiveDate);
                        if (EntityIDFromForm.HasValue)
                        {
                            // Get the entity from the cache, or the Item if not found. It has an ID
                            // so it should exist.
                            if (EntityCache.Values.Any(e => e.EntityID.Value == EntityIDFromForm.Value))
                            {
                                Entity = EntityCache.Values.First(e => e.EntityID.Value == EntityIDFromForm.Value);
                            }
                            else
                            {
                                if (MemberEffectiveDateFieldName == null ||
                                    (MemberEffectiveDateFieldName != null && (DateTime)this.GetType().GetProperty(MemberEffectiveDateFieldName).GetValue(this) == default(DateTime)))
                                {
                                    Entity = BoundItem.AllEntities.Where(e => e.EntityID == EntityIDFromForm).FirstOrDefault();
                                }
                                else
                                {
                                    Entity = EffectiveRecord.AllEntities.Where(e => e.EntityID == EntityIDFromForm).FirstOrDefault();
                                }
                                if (Entity == null)
                                {
                                    Log.Error("Trying to get an entity with ID {0} failed.", EntityIDFromForm.HasValue ? EntityIDFromForm.Value.ToString() : "null");
                                    throw new ArgumentOutOfRangeException("The passed EntityID does not exist.");
                                }
                                EntityCache[Entity.Type] = Entity;
                                if (Direction == Direction.Pull)
                                {
                                    // Set the ID value on the form if we are pulling
                                    this.GetType().GetProperty(MemberIDMemberName).SetValue(this, Entity.EntityID);
                                }
                            }

                        }
                        else
                        {
                            // There's no EntityID set so assume we will create a new one.
                            // This instance is just to access its Type member.
                            EntityBase Instance = EntityBase.GetEntityBySystemType(MemberEntityType);
                            if (EntityCache.ContainsKey(Instance.Type))
                            {
                                Entity = EntityCache[Instance.Type];
                            }
                            else
                            {
                                if (Direction == Direction.Pull)
                                {
                                    // "create" a new entity so we can pull is default values. It's not going to be created,
                                    // it's not goint to be attached to the item and it won't get persisted.
                                    Entity = EffectiveRecord.GetOrCreateEntityButDontSave(Instance.Type, EndEffectiveDate);
                                }
                                else if (Direction == Direction.Push)
                                {
                                    // We are pushing values to the item so we want to create a real entity.
                                    Entity = EffectiveRecord.CreateEntity(Instance.Type, EndEffectiveDate);
                                }
                                if (Entity == null)
                                {
                                    Log.Error("Trying to create an entity of type {0} failed.", Instance.Type.Name);
                                    throw new ArgumentOutOfRangeException("Error trying to create an entity.");
                                }
                                EntityCache[Instance.Type] = Entity;
                                if (Direction == Direction.Pull)
                                {
                                    // Set the ID value on the form if we are pulling
                                    this.GetType().GetProperty(MemberIDMemberName).SetValue(this, Entity.EntityID);
                                }
                            }
                        }

                        if (Entity == null || MemberEntityType != Entity.GetType())
                        {
                            Log.Error("Error trying to bind to form.");
                            throw new InvalidOperationException("Cannot bind to entity.");
                        }

                        // Now we have enough information to actually push or pull the values.

                        if (MemberName == "EffectiveDate") // Special case for this special field
                        {

                            if (Direction == Direction.Push)
                            {
                                if (!FormMembersNotToChange.ContainsKey(MemberName))
                                {
                                    Entity.EffectiveDate = EffectiveDate;
                                }
                                else
                                {
                                    // If an EntityID was sent to copy from, get it and try to copy the value from it.
                                    int PreviousEntityID = FormMembersNotToChange[MemberName];
                                    EntityBase PreviousEntity = BoundItem.AllEntities.Where(e => e.EntityID.HasValue && e.EntityID.Value == PreviousEntityID).FirstOrDefault();
                                    if (PreviousEntity != null)
                                    {
                                        Entity.EffectiveDate = PreviousEntity.EffectiveDate;
                                    }
                                    else
                                    {
                                        Log.Error("Unable to get previous value from the supplied EntityID. MemberName: {0}, PreviousEntityID: {1}.", MemberName, PreviousEntityID);
                                        throw new InvalidOperationException("Unable to pull value from supplied EntityID.");
                                    }
                                }
                            }
                            else if (Direction == Direction.Pull)
                            {
                                SetValueOn(Member, Entity.EffectiveDate);
                            }
                        }
                        else if (MemberName == "EndEffectiveDate") // Special case for this special field
                        {

                            if (Direction == Direction.Push) {
                                if (!FormMembersNotToChange.ContainsKey(MemberName))
                                {
                                    Entity.EndEffectiveDate = EndEffectiveDate;
                                }
                                else
                                {
                                    // If an EntityID was sent to copy from, get it and try to copy the value from it.
                                    int PreviousEntityID = FormMembersNotToChange[MemberName];
                                    EntityBase PreviousEntity = BoundItem.AllEntities.Where(e => e.EntityID.HasValue && e.EntityID.Value == PreviousEntityID).FirstOrDefault();
                                    if (PreviousEntity != null)
                                    {
                                        Entity.EndEffectiveDate = PreviousEntity.EndEffectiveDate;
                                    }
                                    else
                                    {
                                        Log.Error("Unable to get previous value from the supplied EntityID. MemberName: {0}, PreviousEntityID: {1}.", MemberName, PreviousEntityID);
                                        throw new InvalidOperationException("Unable to pull value from supplied EntityID.");
                                    }
                                }
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
                                Log.Error("Binding is not configured properly. MemberName: {0}", MemberName);
                                throw new InvalidOperationException("Binding is not configured properly.");
                            }

                            if (Direction == Direction.Push) {
                                // If the field is not found on this dictionary, it is
                                // safe to set it from the value on the form.
                                if (!FormMembersNotToChange.ContainsKey(MemberName))
                                {
                                    EntityProperty.Value = GetValueFrom(Member);
                                }
                                else
                                {
                                    // If an EntityID was sent to copy from, get it and try to copy the value from it.
                                    int PreviousEntityID = FormMembersNotToChange[MemberName];
                                    EntityBase PreviousEntity = BoundItem.AllEntities.Where(e => e.EntityID.HasValue && e.EntityID.Value == PreviousEntityID).FirstOrDefault();
                                    if (PreviousEntity != null)
                                    {
                                        var PreviousEntityProperty = (IField)(MemberEntityType.GetProperty(MemberName).GetValue(PreviousEntity));
                                        EntityProperty.Value = PreviousEntityProperty.Value;
                                    }
                                    else
                                    {
                                        Log.Error("Unable to get previous value from the supplied EntityID. MemberName: {0}, PreviousEntityID: {1}.", MemberName, PreviousEntityID);
                                        throw new InvalidOperationException("Unable to pull value from supplied EntityID.");
                                    }
                                }
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

        private IEnumerable<MemberInfo> GetBoundFields()
        {
            List<MemberInfo> MemberInfos = new List<MemberInfo>();

            var Properties = this.GetType().GetProperties().Where(p => p.CustomAttributes.Any(a => typeof(BindAttribute).IsAssignableFrom(a.AttributeType)));
            var Fields = this.GetType().GetFields().Where(p => p.CustomAttributes.Any(a => typeof(BindAttribute).IsAssignableFrom(a.AttributeType)));

            MemberInfos.AddRange(Properties);
            MemberInfos.AddRange(Fields);

            return MemberInfos;
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
                Log.Error("An invalid Member was sent to Form.SetValueOn. MemberType: {0}", Member.MemberType.ToString());
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

            Log.Error("An invalid Member was sent to Form.GetValueFrom. MemberType: {0}", Member.MemberType.ToString());
            throw new ArgumentException("Can only set value on a Property or Field.");
        }
    }
}
