using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Core.Models.Entities
{
    public class EntityType
    {
        public int Value { get; private set; }
        public string Name { get; private set; }
        public Type Type { get; private set; }
        private EntityType(string Name, int Value, Type Type)
        {
            this.Value = Value;
            this.Name = Name;
            this.Type = Type;
        }

        public static readonly EntityType Job = new EntityType(Strings.Job, 1, typeof(JobEntity));
        public static readonly EntityType Address = new EntityType(Strings.Job, 2, typeof(AddressEntity));
        public static readonly EntityType Employee_General = new EntityType(Strings.Employee_General, 3, typeof(EmployeeGeneralEntity));

        public static implicit operator int (EntityType dt)
        {
            return dt.Value;
        }

        public static explicit operator EntityType(int i)
        {
            switch (i)
            {
                case 1:
                    return Job;
                case 2:
                    return Address;
                case 3:
                    return Employee_General;
                default:
                    throw new InvalidCastException(string.Format("Cannot convert the int value {0} to an EmployeeEntity instance.", i));
            }
        }
    }
}
