using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Core.Models.Entities;
using HRMS.Core.Services;

namespace HRMS.Core.Models
{
    public class Employee
    {
        public IEnumerable<EntityBase> AllEntities { get; private set; }
        private Dictionary<EntityType, IEnumerable<EntityBase>> AllEntitiesByType;
        public int? EmployeeID { get; private set; }

        private readonly IPersistenceService PersistenceService;

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

        public Employee(int EmployeeID, IPersistenceService PersistenceService, bool LoadEmployee = true)
        {
            this.EmployeeID = EmployeeID;
            this.PersistenceService = PersistenceService;
            if (LoadEmployee)
            {
                this.LoadByID(EmployeeID);
            }
        }

        public void Load()
        {
            if (!EmployeeID.HasValue)
            {
                throw new InvalidOperationException("Cannot reload an employee with a null ID.");
            }
            LoadByID(EmployeeID.Value);
        }

        private void LoadByID(int EmployeeID)
        {
            this.EmployeeID = EmployeeID;
            var EmployeeRecordsList = PersistenceService.RetreiveAllEmployeeRecords(this);

            EmployeeRecords = new SortedDictionary<DateTime, EmployeeRecord>();

            foreach (var EmployeeRecord in EmployeeRecordsList)
            {
                EmployeeRecords[EmployeeRecord.EffectiveDate] = EmployeeRecord;
            }
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
