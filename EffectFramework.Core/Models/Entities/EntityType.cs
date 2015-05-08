using System;
using System.Collections.Generic;

namespace EffectFramework.Core.Models.Entities
{
    public abstract class EntityType
    {
        public int Value { get; protected set; }
        public string Name { get; protected set; }
        public Type Type { get; protected set; }
        private static Dictionary<int, EntityType> TypeRegistry = new Dictionary<int, EntityType>();
        protected EntityType(string Name, int Value, Type Type)
        {
            this.Value = Value;
            this.Name = Name;
            this.Type = Type;
            RegisterType(this);
        }

        public static implicit operator int (EntityType dt)
        {
            return dt.Value;
        }

        public static explicit operator EntityType(int i)
        {
            if (TypeRegistry.ContainsKey(i))
            {
                return TypeRegistry[i];
            }
            throw new InvalidCastException(string.Format("Cannot convert the int value {0} to an EntityType instance.", i));
        }

        private static void RegisterType(EntityType Type)
        {
            TypeRegistry[Type.Value] = Type;
        }
    }
}
