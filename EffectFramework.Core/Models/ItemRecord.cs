using System;
using System.Collections.Generic;
using System.Linq;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Services;

namespace EffectFramework.Core.Models
{
    public class ItemRecord
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

        public int? ItemRecordID { get; private set; }
        public DateTime EffectiveDate { get; private set; }
        public DateTime? EndEffectiveDate { get; private set; }
        public Guid Guid { get; private set; }
        public bool Dirty { get; private set; }
        public int ItemID { get; private set; }

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

        public ItemRecord(IPersistenceService PersistenceService)
        {
            this.PersistenceService = PersistenceService;
            this.Dirty = true;
        }

        public ItemRecord(Db.ItemRecord DbItemRecord, IPersistenceService PersistenceService)
        {
            if (DbItemRecord == null)
            {
                throw new ArgumentNullException();
            }
            if (!DbItemRecord.ItemRecordID.HasValue)
            {
                throw new ArgumentException("Cannot create record with a null ID.");
            }

            this.Dirty = false;
            this.ItemRecordID = DbItemRecord.ItemRecordID.Value;
            this.PersistenceService = PersistenceService;
            this.LoadByDbRecord(DbItemRecord);
        }

        public ItemRecord(int ItemRecordID)
        {
            this.Dirty = false;
            this.ItemRecordID = ItemRecordID;
            this.LoadByID(ItemRecordID);
        }

        private void LoadByID(int ItemRecordID)
        {
            Db.ItemRecord DbItemRecord = PersistenceService.RetreiveSingleDbItemRecord(ItemRecordID);
            LoadByDbRecord(DbItemRecord);
        }

        private void LoadByDbRecord(Db.ItemRecord DbItemRecord, bool LoadEntities = true)
        {
            this.ItemRecordID = DbItemRecord.ItemRecordID;
            this.EffectiveDate = DbItemRecord.EffectiveDate;
            this.EndEffectiveDate = DbItemRecord.EndEffectiveDate;
            this.Guid = DbItemRecord.Guid;
            this.ItemID = DbItemRecord.ItemID;
            
            if (LoadEntities)
            {
                this.LoadEntities();
            }
        }

        private void LoadEntities()
        {
            if (!this.ItemRecordID.HasValue)
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

        public void CopyEntitiesFrom(ItemRecord OtherRecord)
        {
            this.Dirty = true;
            // Shallow clone
            AllEntitiesByType = new Dictionary<EntityType, List<EntityBase>>(OtherRecord.AllEntitiesByType);
        }
    }
}
