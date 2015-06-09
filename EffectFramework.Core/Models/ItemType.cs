using System;
using System.Collections.Generic;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// All custom item types must inherit from this base class.
    /// </summary>
    [Serializable]
    public class ItemType
    {
        /// <summary>
        /// Gets the Item Type ID for this item type.
        /// </summary>
        /// <value>
        /// The Item Type ID.
        /// </value>
        public int Value { get; private set; }

        /// <summary>
        /// Gets the name of the item type.
        /// </summary>
        /// <value>
        /// The name of the item type.
        /// </value>
        public string Name { get; private set; }
        public Type Type { get; private set; }
        private static Dictionary<int, ItemType> TypeRegistry = new Dictionary<int, ItemType>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemType"/> class. The name and value should
        /// match the name and value in the data store (keeping these up to date is not a feature
        /// of this framework). The Type parameter is the type of the class used to model the particular
        /// Item.
        /// </summary>
        /// <param name="Name">The name of the item.</param>
        /// <param name="Value">The ItemTypeID in the data store.</param>
        /// <param name="Type">The type of the model class.</param>
        protected ItemType(string Name, int Value, Type Type)
        {
            this.Value = Value;
            this.Name = Name;
            this.Type = Type;
            RegisterType(this);
        }


        public static implicit operator int (ItemType dt)
        {
            return dt.Value;
        }

        public static explicit operator ItemType(int i)
        {
            if (TypeRegistry.ContainsKey(i))
            {
                return TypeRegistry[i];
            }
            throw new InvalidCastException(string.Format("Cannot convert the int value {0} to a FieldType instance.", i));
        }

        private static void RegisterType(ItemType Type)
        {
            if (TypeRegistry.ContainsKey(Type.Value))
            {
                throw new InvalidOperationException("Cannot register the same Item Type twice.");
            }
            TypeRegistry[Type.Value] = Type;
        }

        public static bool operator ==(ItemType It1, ItemType It2)
        {
            if ((object)It1 == null && (object)It2 == null)
            {
                return true;
            }
            if ((object)It1 == null || (object)It2 == null)
            {
                return false;
            }

            return It1.Value == It2.Value;
        }

        public static bool operator !=(ItemType It1, ItemType It2)
        {
            if ((object)It1 == null && (object)It2 == null)
            {
                return false;
            }
            if ((object)It1 == null || (object)It2 == null)
            {
                return true;
            }

            return It1.Value != It2.Value;
        }
    }
}
