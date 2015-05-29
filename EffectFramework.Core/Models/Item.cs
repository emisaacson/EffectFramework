using System;
using System.Linq;
using System.Collections.Generic;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Services;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// All items must inherit this base class.
    /// </summary>
    public abstract class Item
    {
        /// <summary>
        /// Gets all the non-deleted entityies for this particular item.
        /// </summary>
        /// <value>
        /// All non-deleted entities for this Item.
        /// </value>
        public IEnumerable<EntityBase> AllEntities
        {
            get
            {
                return _AllEntities.Where(e => !e.FlagForRemoval && !e.IsDeleted);
            }
        }
        private List<EntityBase> _AllEntities = new List<EntityBase>();
        /// <summary>
        /// Gets or sets the item identifier.
        /// </summary>
        /// <value>
        /// The item identifier.
        /// </value>
        public int? ItemID { get; protected set; }
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid Guid { get; protected set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Item"/> is in sync with the data store.
        /// </summary>
        /// <value>
        ///   <c>true</c> if dirty; otherwise, <c>false</c>.
        /// </value>
        public bool Dirty { get; protected set; }
        /// <summary>
        /// Gets the type of this item.
        /// </summary>
        /// <value>
        /// The ItemType of this item.
        /// </value>
        public abstract ItemType Type { get; }

        protected readonly IPersistenceService PersistenceService;

        /// <summary>
        /// Gets an entity collection representing all entities overlapping with the current EffectiveDate
        /// set on this item.
        /// </summary>
        /// <value>
        /// An entity collection representing all entities overlapping with the current EffectiveDate
        /// set on this item.
        /// </value>
        public EntityCollection EffectiveRecord
        {
            get
            {
                return GetEntityCollectionForDate(EffectiveDate);
            }
        }

        protected DateTime _EffectiveDate = DateTime.Now.Date;
        /// <summary>
        /// Gets or sets the current effective date of this item.
        /// </summary>
        /// <value>
        /// The effective date.
        /// </value>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class. The item is initially
        /// dirty and has no item ID. When persisted, an ItemID will be added to the class.
        /// </summary>
        /// <param name="PersistenceService">The persistence service (for DI injection).</param>
        public Item(IPersistenceService PersistenceService)
        {
            this.Dirty = true;
            this.PersistenceService = PersistenceService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="ItemID">The item identifier.</param>
        /// <param name="PersistenceService">The persistence service.</param>
        /// <param name="LoadItem">if set to <c>true</c>, retreive all data for this item from the PersistenceService.</param>
        /// <param name="Sparse">if set to <c>true</c>, only load entities for the current EffectiveRecord (unimplemented at the moment).</param>
        public Item(int ItemID, IPersistenceService PersistenceService, bool LoadItem = true, bool Sparse = false)
        {
            this.ItemID = ItemID;
            this.PersistenceService = PersistenceService;
            if (LoadItem)
            {
                this.LoadByID(ItemID, Sparse);
            }
        }

        /// <summary>
        /// Gets an entity collection for all entities overlapping with the passed Effective Date.
        /// </summary>
        /// <param name="EffectiveDate">The effective date.</param>
        /// <returns>An entity collection for all entities overlapping with the passed Effective Date.</returns>
        public EntityCollection GetEntityCollectionForDate(DateTime EffectiveDate)
        {
            return new EntityCollection(this, EffectiveDate, PersistenceService);
        }

        /// <summary>
        /// Reload all data from the database, getting a fresh copy. This method will fail if the Item
        /// has not yet been persisted to the data store.
        /// </summary>
        /// <param name="Sparse">if set to <c>true</c>, only load entities overlapping with the current EffectiveDate (unimplemented).</param>
        /// <exception cref="System.InvalidOperationException">Cannot reload an item with a null ID.</exception>
        public void Load(bool Sparse = false)
        {
            if (!ItemID.HasValue)
            {
                throw new InvalidOperationException("Cannot reload an item with a null ID.");
            }
            LoadByID(ItemID.Value, Sparse);
        }


        /// <summary>
        /// Persist the item and all of its entities to database.
        /// </summary>
        /// <param name="ctx">The database context. If one is not provided, a new one will be created with a transaction.</param>
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

        internal void PerformUpdate(EntityBase Entity, IUpdateStrategy PreferredUpdateStrategy = null, IUpdateStrategy PreferredUpdateStrategyForDuplicateDates = null)
        {
            Entity.GetUpdatePolicy().PerformUpdate(this, Entity, PreferredUpdateStrategy, PreferredUpdateStrategyForDuplicateDates);
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
