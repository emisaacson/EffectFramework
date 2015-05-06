using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Core.Models.Entities;
using HRMS.Core.Models.Fields;
using HRMS.Core.Services;
using Ninject;

namespace HRMS.Core.Models
{
    public class EmployeeRecord
    {
        public IEnumerable<EntityBase> AllEntities {
            get
            {
                List<EntityBase> Output = new List<EntityBase>();
                foreach (EntityType Entity in AllEntitiesByType.Keys)
                {
                    Output.AddRange(AllEntitiesByType[Entity]);
                }
                return Output;
            }
        }

        private readonly IPersistenceService PersistenceService;

        public int? EmployeeRecordID { get; private set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }

        private Dictionary<EntityType, List<EntityBase>> AllEntitiesByType = new Dictionary<EntityType, List<EntityBase>>();

        public EntityT GetFirstEntityOrDefault<EntityT>() where EntityT : EntityBase, new()
        {
            EntityT Instance = new EntityT();
            if (!AllEntitiesByType.ContainsKey(Instance.Type))
            {
                return default(EntityT);
            }

            return (EntityT)AllEntitiesByType[Instance.Type].First();
        }

        public IEnumerable<EntityT> GetAllEntitiesOrDefault<EntityT>() where EntityT : EntityBase, new()
        {
            EntityT Instance = new EntityT();
            if (!AllEntitiesByType.ContainsKey(Instance.Type))
            {
                return null;
            }

            return (IEnumerable<EntityT>)AllEntitiesByType[Instance.Type];
        }

        public EmployeeRecord(IPersistenceService PersistenceService)
        {
            this.PersistenceService = PersistenceService;
        }

        public EmployeeRecord(Db.EmployeeRecord DbEmployeeRecord, IPersistenceService PersistenceService)
        {
            if (DbEmployeeRecord == null)
            {
                throw new ArgumentNullException();
            }
            if (!DbEmployeeRecord.EmployeeRecordID.HasValue)
            {
                throw new ArgumentException("Cannot create record with a null ID.");
            }

            this.EmployeeRecordID = DbEmployeeRecord.EmployeeRecordID.Value;
            this.PersistenceService = PersistenceService;
            this.LoadByDbRecord(DbEmployeeRecord);
        }

        public EmployeeRecord(int EmployeeRecordID)
        {
            this.EmployeeRecordID = EmployeeRecordID;
            this.LoadByID(EmployeeRecordID);
        }

        private void LoadByID(int EmployeeRecordID)
        {
            Db.EmployeeRecord DbEmployeeRecord = PersistenceService.RetreiveSingleDbEmployeeRecord(EmployeeRecordID);
            LoadByDbRecord(DbEmployeeRecord);
        }

        private void LoadByDbRecord(Db.EmployeeRecord DbEmployeeRecord, bool LoadEntities = true)
        {
            this.EmployeeRecordID = DbEmployeeRecord.EmployeeRecordID;
            this.EffectiveDate = DbEmployeeRecord.EffectiveDate;
            this.EndEffectiveDate = DbEmployeeRecord.EndEffectiveDate;
            
            if (LoadEntities)
            {
                this.LoadEntities();
            }
        }

        private void LoadEntities()
        {
            if (!this.EmployeeRecordID.HasValue)
            {
                throw new InvalidOperationException("Cannot load entities if the record ID is not known.");
            }

            var Entities = PersistenceService.RetreiveAllEntities(this);

            foreach (var Entity in Entities)
            {
                if (AllEntitiesByType.ContainsKey(Entity.Type))
                {
                    AllEntitiesByType[Entity.Type].Add(Entity);
                }
                else
                {
                    AllEntitiesByType[Entity.Type] = new List<EntityBase> { Entity };
                }
            }
        }
    }
}
