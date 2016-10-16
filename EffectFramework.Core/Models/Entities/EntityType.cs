using System;
using System.Collections.Generic;
using System.Linq;

namespace EffectFramework.Core.Models.Entities
{
    [Serializable]
    public abstract class EntityType
    {
        public long Value { get; protected set; }
        public string Name { get; protected set; }
        public Type Type { get; protected set; }
        public virtual long TenantID
        {
            get {
                return Configure.GetTenantResolutionProvider().GetTenantID();
            }
        }

        private static Dictionary<long, EntityType> TypeRegistry = new Dictionary<long, EntityType>();
        protected EntityType(string Name, long Value, Type Type)
        {
            this.Value = Value;
            this.Name = Name;
            this.Type = Type;
            RegisterType(this);
        }

        public static implicit operator long (EntityType dt)
        {
            return dt.Value;
        }

        public static explicit operator EntityType(long i)
        {
            if (TypeRegistry.ContainsKey(i))
            {
                return TypeRegistry[i];
            }
            throw new InvalidCastException(string.Format("Cannot convert the long value {0} to an EntityType instance.", i));
        }

        public static explicit operator EntityType(string s)
        {
            var Exists = TypeRegistry.Any(t => t.Value.Name == s);
            if (Exists)
            {
                return TypeRegistry.First(t => t.Value.Name == s).Value;
            }
            throw new InvalidCastException(string.Format("Cannot convert the string value {0} to an EntityType instance.", s));
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

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
