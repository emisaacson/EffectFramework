﻿using System;
using System.Collections.Generic;
using System.Linq;
using HRMS.Core.Models.Entities;
using HRMS.Core.Services;

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
        public DateTime EffectiveDate { get; private set; }
        public DateTime? EndEffectiveDate { get; private set; }
        public Guid Guid { get; private set; }
        public bool Dirty { get; private set; }

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

        public void SetEffectiveDate(DateTime EffectiveDate)
        {
            this.Dirty = true;
            this.EffectiveDate = EffectiveDate;
        }

        public void SetEndEffectiveDate(DateTime? EndEffectiveDate)
        {
            this.Dirty = true;
            this.EndEffectiveDate = EndEffectiveDate;
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
            this.Dirty = true;
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

            this.Dirty = false;
            this.EmployeeRecordID = DbEmployeeRecord.EmployeeRecordID.Value;
            this.PersistenceService = PersistenceService;
            this.LoadByDbRecord(DbEmployeeRecord);
        }

        public EmployeeRecord(int EmployeeRecordID)
        {
            this.Dirty = false;
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
            this.Guid = DbEmployeeRecord.Guid;
            
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

        public void CopyEntitiesFrom(EmployeeRecord OtherRecord)
        {
            this.Dirty = true;
            AllEntitiesByType = OtherRecord.AllEntitiesByType;
        }
    }
}
