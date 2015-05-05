using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Core.Models.Entities;

namespace HRMS.Core.Models
{
    public class Employee
    {
        public IEnumerable<IEntity> AllEntities { get; private set; }
        private Dictionary<EntityType, IEnumerable<IEntity>> AllEntitiesByType;

        public SortedDictionary<DateTime, EmployeeRecord> EmployeeRecords { get; private set; }

        private DateTime _EffectiveDate = DateTime.Now;
        public DateTime EffectiveDate {
            get
            {
                return this._EffectiveDate;
            }
            private set
            {
                this._EffectiveDate = value;
            }
        }

        public Employee()
        {

        }

        public Employee(int EmployeeID)
        {
            this.LoadByID(EmployeeID);
        }

        private void LoadByID(int employeeID)
        {
            
        }

        public void ChangeEffectiveDate(DateTime EffectiveDate)
        {
            if (EffectiveDate == null)
            {
                throw new ArgumentNullException();
            }

            this.EffectiveDate = EffectiveDate;
        }
    }
}
