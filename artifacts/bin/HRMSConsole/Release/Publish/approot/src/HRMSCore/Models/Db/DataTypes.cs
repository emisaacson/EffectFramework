using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Core.Models.Db
{
    public class DataTypes
    {
        private int Value;
        private DataTypes(int Value)
        {
            this.Value = Value;
        }

        public static readonly DataTypes Text = new DataTypes(1);
        public static readonly DataTypes Date = new DataTypes(2);
        public static readonly DataTypes Decimal = new DataTypes(3);
        public static readonly DataTypes Boolean = new DataTypes(4);
        public static readonly DataTypes Person = new DataTypes(5);

        public static implicit operator int (DataTypes dt)
        {
            return dt.Value;
        }

        public static explicit operator DataTypes (int i)
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
                default:
                    throw new InvalidCastException(string.Format("Cannot convert the int value {0} to a DataTypes instance.", i));
            }
        }
    }
}
