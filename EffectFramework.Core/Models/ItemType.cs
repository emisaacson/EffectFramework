using System;
using System.Collections.Generic;

namespace EffectFramework.Core.Models
{
    public class ItemType
    {
        public int Value { get; private set; }
        public string Name { get; private set; }
        public Type Type { get; private set; }
        private static Dictionary<int, ItemType> TypeRegistry = new Dictionary<int, ItemType>();

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
            TypeRegistry[Type.Value] = Type;
        }
    }
}
