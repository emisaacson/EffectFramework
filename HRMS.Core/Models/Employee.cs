using System;
using System.Linq;
using System.Collections.Generic;
using HRMS.Core.Models.Entities;
using HRMS.Core.Services;

namespace HRMS.Core.Models
{
    public class Employee
    {
        public IEnumerable<EntityBase> AllEntities { get; private set; }
        private Dictionary<EntityType, IEnumerable<EntityBase>> AllEntitiesByType;
        public int? EmployeeID { get; private set; }
        public Guid Guid { get; protected set; }

        private readonly IPersistenceService PersistenceService;

        public SortedDictionary<DateTime, EmployeeRecord> EmployeeRecords { get; private set; }
        public EmployeeRecord EffectiveRecord
        {
            get
            {
                return GetEffectiveRecordForDate(EffectiveDate);
            }
        }

        private EmployeeRecord GetEffectiveRecordForDate(DateTime EffectiveDate)
        {
            var _EffectiveRecord = EmployeeRecords
                .Where(e =>
                    e.Key <= this.EffectiveDate &&
                    (!e.Value.EndEffectiveDate.HasValue || e.Value.EndEffectiveDate > EffectiveDate))
                .FirstOrDefault();

            if (!_EffectiveRecord.Equals(default(KeyValuePair<DateTime, EmployeeRecord>)) && _EffectiveRecord.Value != null)
            {
                return _EffectiveRecord.Value;
            }

            return null;
        }

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

        public Employee(IPersistenceService PersistenceService)
        {
            this.PersistenceService = PersistenceService;
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

        public void LoadByID(int EmployeeID)
        {
            if (this.EmployeeID.HasValue && EmployeeID != this.EmployeeID.Value)
            {
                throw new InvalidOperationException("Please do not reuse the same employee object for an employee with a different ID.");
            }
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
            if (EffectiveDate == null || EffectiveDate == default(DateTime))
            {
                throw new ArgumentNullException();
            }

            this.EffectiveDate = EffectiveDate;
        }

        //public EmployeeRecord GetOrCreateEffectiveDateRange(DateTime EffectiveDate)
        //{
        //    if (EffectiveDate == null || EffectiveDate == default(DateTime))
        //    {
        //        throw new ArgumentNullException();
        //    }

        //    if (EmployeeRecords.ContainsKey(EffectiveDate))
        //    {
        //        return EmployeeRecords[EffectiveDate];
        //    }

        //    if (EmployeeRecords.Count > 0 && EmployeeRecords.First().Key /*The oldest date*/ < EffectiveDate)
        //    {
        //        EmployeeRecord PreviousEmployeeRecord = GetEffectiveRecordForDate(EffectiveDate);
        //        EmployeeRecord New
        //    }
        //    else
        //    {
        //        EmployeeRecords[EffectiveDate] = new EmployeeRecord(PersistenceService);
        //        return EmployeeRecords[EffectiveDate];
        //    }
        //}
    }
}
