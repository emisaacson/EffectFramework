using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Core.Models.Entities
{
    public class EmployeeEntity
    {
        protected int Value;
        private EmployeeEntity(int Value)
        {
            this.Value = Value;
        }

        public static readonly EmployeeEntity Job = new EmployeeEntity(1);
        public static readonly EmployeeEntity Address = new EmployeeEntity(2);
        public static readonly EmployeeEntity EmployeeGeneral = new EmployeeEntity(3);

        public static implicit operator int (EmployeeEntity dt)
        {
            return dt.Value;
        }

        public static explicit operator EmployeeEntity(int i)
        {
            switch (i)
            {
                case 1:
                    return Job;
                case 2:
                    return Address;
                case 3:
                    return EmployeeGeneral;
                default:
                    throw new InvalidCastException(string.Format("Cannot convert the int value {0} to an Entity instance.", i));
            }
        }
    }
}
