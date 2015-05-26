using System;
using System.Linq;
using System.Collections.Generic;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Services;

namespace EffectFramework.Core.Models
{
    public abstract class Item
    {
        public IEnumerable<EntityBase> AllEntities
        {
            get
            {
                return _AllEntities.Where(e => !e.FlagForRemoval && !e.IsDeleted);
            }
        }
        private List<EntityBase> _AllEntities = new List<EntityBase>();
        public int? ItemID { get; protected set; }
        public Guid Guid { get; protected set; }
        public bool Dirty { get; protected set; }
        public abstract ItemType Type { get; }

        protected readonly IPersistenceService PersistenceService;
        
        public EntityCollection EffectiveRecord
        {
            get
            {
                return GetEntityCollectionForDate(EffectiveDate);
            }
        }

        protected DateTime _EffectiveDate = DateTime.Now.Date;
        public DateTime EffectiveDate {
            get
            {
                return this._EffectiveDate;
            }
            set
            {
                this._EffectiveDate = value;
            }
        }

        public Item(IPersistenceService PersistenceService)
        {
            this.Dirty = true;
            this.PersistenceService = PersistenceService;
        }

        public Item(int ItemID, IPersistenceService PersistenceService, bool LoadItem = true, bool Sparse = false)
        {
            this.ItemID = ItemID;
            this.PersistenceService = PersistenceService;
            if (LoadItem)
            {
                this.LoadByID(ItemID, Sparse);
            }
        }

        public EntityCollection GetEntityCollectionForDate(DateTime EffectiveDate)
        {
            return new EntityCollection(this, EffectiveDate, PersistenceService);
        }

        public void Load(bool Sparse = false)
        {
            if (!ItemID.HasValue)
            {
                throw new InvalidOperationException("Cannot reload an item with a null ID.");
            }
            LoadByID(ItemID.Value, Sparse);
        }


        public void PersistToDatabase(Db.IDbContext ctx = null)
        {
            Db.IDbContext db = null;
            try {
                if (ctx == null)
                {
                    db = PersistenceService.GetDbContext();
                    db.BeginTransaction();
                }
                else
                {
                    db = ctx;
                }

                var Identity = PersistenceService.SaveSingleItem(this, db);
                this.ItemID = Identity.ObjectID;
                this.Guid = Identity.ObjectGuid;

                var __AllEntities = _AllEntities.ToArray();
                for (int i = 0; i < __AllEntities.Count(); i++)
                {
                    if (__AllEntities[i].Dirty)
                    {
                        PerformUpdate(__AllEntities[i]);
                    }
                }

                foreach (var Entity in _AllEntities)
                {
                    Entity.PersistToDatabase(this, db);
                }
                RemoveDeadEntities();

                this.Dirty = false;

                if (ctx == null)
                {
                    db.Commit();
                }
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }
            }
        }
        protected void LoadByID(int ItemID, bool Sparse = false)
        {
            if (this.ItemID.HasValue && ItemID != this.ItemID.Value)
            {
                throw new InvalidOperationException("Please do not reuse the same item object for an Item with a different ID.");
            }
            this.ItemID = ItemID;
            this.Guid = PersistenceService.RetreiveGuidForItem(this);

            DateTime? DateToSend = EffectiveDate;

            _AllEntities = PersistenceService.RetreiveAllEntities(this, Sparse ? DateToSend : null);
            foreach (var Entity in _AllEntities)
            {
                Entity.Item = this;
            }

            this.Dirty = false;
        }

        internal void AddEntity(EntityBase Entity)
        {
            if (Entity == null)
            {
                throw new ArgumentNullException();
            }

            _AllEntities.Add(Entity);
            Entity.Item = this;

            //PerformUpdate(Entity);
        }

        internal void PerformUpdate(EntityBase Entity)
        {
            Entity.GetUpdatePolicy().PerformUpdate(this, Entity);
        }

        internal void RemoveEntity(EntityBase Entity)
        {
            if (Entity == null)
            {
                throw new ArgumentNullException();
            }

            _AllEntities.Remove(Entity);
        }

        internal void RemoveDeadEntities()
        {
            _AllEntities.RemoveAll(x => x.FlagForRemoval);
        }
    }
}
