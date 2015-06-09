using System;
using System.Collections.Generic;

namespace EffectFramework.Core.Models.Fields
{
    /// <summary>
    /// The base class all fields must inherit from.
    /// </summary>
    [Serializable]
    public class FieldType
    {
        /// <summary>
        /// Gets or sets the field ID to match the persistence service..
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public int Value { get; protected set; }
        public DataType DataType { get; protected set; }
        public string Name { get; protected set; }
        public int? LookupTypeID { get; protected set; }
        private static Dictionary<int, FieldType> TypeRegistry = new Dictionary<int, FieldType>();

        protected FieldType(string Name, int Value, DataType DataType, int? LookupTypeID = null)
        {
            this.Name = Name;
            this.Value = Value;
            this.DataType = DataType;
            this.LookupTypeID = LookupTypeID;
            RegisterType(this);
        }

        public static implicit operator int (FieldType dt)
        {
            return dt.Value;
        }

        public static explicit operator FieldType(int i)
        {
            if (TypeRegistry.ContainsKey(i))
            {
                return TypeRegistry[i];
            }
            throw new InvalidCastException(string.Format("Cannot convert the int value {0} to a FieldType instance.", i));
        }

        private static void RegisterType(FieldType Type)
        {
            if (TypeRegistry.ContainsKey(Type.Value))
            {
                throw new InvalidOperationException("Cannot register the same Field Type twice.");
            }
            TypeRegistry[Type.Value] = Type;
        }

        public static bool operator ==(FieldType Ft1, FieldType Ft2)
        {
            if ((object)Ft1 == null && (object)Ft2 == null)
            {
                return true;
            }
            if ((object)Ft1 == null || (object)Ft2 == null)
            {
                return false;
            }

            return Ft1.Value == Ft2.Value;
        }

        public static bool operator !=(FieldType Ft1, FieldType Ft2)
        {
            if ((object)Ft1 == null && (object)Ft2 == null)
            {
                return false;
            }
            if ((object)Ft1 == null || (object)Ft2 == null)
            {
                return true;
            }

            return Ft1.Value != Ft2.Value;
        }
    }
}
