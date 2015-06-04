using EffectFramework.Core.Models.Entities;
using System;

namespace EffectFramework.Core.Models.Fields
{
    /// <summary>
    /// Each DataType gets an ID and an instance on this class
    /// </summary>
    public class DataType
    {
        public int Value { get; private set; }
        public Type ValueType { get; private set; }
        public Type DereferencedType { get; private set; }

        private DataType(int Value, Type ValueType, Type DereferencedType)
        {
            this.Value = Value;
            this.ValueType = ValueType;
            this.DereferencedType = DereferencedType;
        }

        public static readonly DataType Text = new DataType(1, typeof(string), typeof(string));
        public static readonly DataType Date = new DataType(2, typeof(DateTime?), typeof(DateTime?));
        public static readonly DataType Decimal = new DataType(3, typeof(decimal?), typeof(decimal?));
        public static readonly DataType Boolean = new DataType(4, typeof(bool?), typeof(bool?));
        public static readonly DataType Lookup = new DataType(5, typeof(int?), typeof(string));
        public static readonly DataType Binary = new DataType(6, typeof(byte[]), typeof(byte[]));
        public static readonly DataType ItemReference = new DataType(7, typeof(int?), typeof(Item));
        public static readonly DataType EntityReference = new DataType(8, typeof(int?), typeof(EntityBase));

        public static implicit operator int (DataType dt)
        {
            return dt.Value;
        }

        public static explicit operator DataType(int i)
        {
            switch (i)
            {
                case 1:
                    return Text;
                case 2:
                    return Date;
                case 3:
                    return Decimal;
                case 4:
                    return Boolean;
                case 5:
                    return Lookup;
                case 6:
                    return Binary;
                case 7:
                    return ItemReference;
                case 8:
                    return EntityReference;
                default:
                    throw new InvalidCastException(string.Format("Cannot convert the int value {0} to a DataType instance.", i));
            }
        }
    }
}
