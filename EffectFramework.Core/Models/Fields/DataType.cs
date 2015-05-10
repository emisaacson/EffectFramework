using System;

namespace EffectFramework.Core.Models.Fields
{
    public class DataType
    {
        public int Value { get; private set; }

        private DataType(int Value)
        {
            this.Value = Value;
        }

        public static readonly DataType Text = new DataType(1);
        public static readonly DataType Date = new DataType(2);
        public static readonly DataType Decimal = new DataType(3);
        public static readonly DataType Boolean = new DataType(4);
        public static readonly DataType Person = new DataType(5);
        public static readonly DataType Binary = new DataType(6);

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
                    return Person;
                case 6:
                    return Binary;
                default:
                    throw new InvalidCastException(string.Format("Cannot convert the int value {0} to a DataTypes instance.", i));
            }
        }
    }
}
