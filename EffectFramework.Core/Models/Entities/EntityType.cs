using System;
using System.Collections.Generic;

namespace EffectFramework.Core.Models.Entities
{
    [Serializable]
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
            if (TypeRegistry.ContainsKey(Type.Value))
            {
                throw new InvalidOperationException("Cannot register the same Entity Type twice.");
            }
            TypeRegistry[Type.Value] = Type;
        }

        public static bool operator ==(EntityType Et1, EntityType Et2)
        {
            if ((object)Et1 == null && (object)Et2 == null)
            {
                return true;
            }
            if ((object)Et1 == null || (object)Et2 == null)
            {
                return false;
            }

            return Et1.Value == Et2.Value;
        }

        public static bool operator !=(EntityType Et1, EntityType Et2)
        {
            if ((object)Et1 == null && (object)Et2 == null)
            {
                return false;
            }
            if ((object)Et1 == null || (object)Et2 == null)
            {
                return true;
            }

            return Et1.Value != Et2.Value;
        }
    }
}
