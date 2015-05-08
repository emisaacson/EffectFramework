using System;
using System.Collections.Generic;

namespace EffectFramework.Core.Models.Fields
{
    public class FieldType
    {
        public int Value { get; protected set; }
        public DataType DataType { get; protected set; }
        public string Name { get; protected set; }
        private static Dictionary<int, FieldType> TypeRegistry = new Dictionary<int, FieldType>();

        protected FieldType(string Name, int Value, DataType DataType)
        {
            this.Name = Name;
            this.Value = Value;
            this.DataType = DataType;
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
            TypeRegistry[Type.Value] = Type;
        }
    }
}
