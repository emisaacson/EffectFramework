using System;
using System.Linq;
using System.Collections.Generic;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Services;
using Ninject;

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
        public bool Sparse { get; protected set; }
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
        public Item(IPersistenceService PersistenceService, bool Sparse = false)
        {
            this.Dirty = true;
            this.Sparse = Sparse;
            this.PersistenceService = PersistenceService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="ItemID">The item identifier.</param>
        /// <param name="PersistenceService">The persistence service.</param>
        /// <param name="LoadItem">if set to <c>true</c>, retreive all data for this item from the PersistenceService.</param>
        /// <param name="Sparse">if set to <c>true</c>, only load entities for the current EffectiveRecord.</param>
        public Item(int ItemID, IPersistenceService PersistenceService, bool LoadItem = true, bool Sparse = false)
        {
            this.ItemID = ItemID;
            this.Sparse = Sparse;
            this.PersistenceService = PersistenceService;
            if (LoadItem)
            {
                this.LoadByID(ItemID);
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
        /// <exception cref="System.InvalidOperationException">Cannot reload an item with a null ID.</exception>
        public void Load()
        {
            if (!ItemID.HasValue)
            {
                throw new InvalidOperationException("Cannot reload an item with a null ID.");
            }
            LoadByID(ItemID.Value);
        }

        public void LoadByView(int ItemID, IEnumerable<Db.CompleteItem> View)
        {
            if (this.ItemID.HasValue && this.ItemID.Value != ItemID)
            {
                throw new InvalidOperationException("Item IDs do not match");
            }

            this.ItemID = ItemID;

            _AllEntities.Clear();

            Db.CompleteItem[] Rows = View.Where(v => v.ItemID == ItemID).ToArray();
            var EntityIDs = View.Select(v => v.EntityID).Distinct();

            foreach (var EntityID in EntityIDs)
            {
                var EntityRows = Rows.Where(r => r.EntityID == EntityID);
                if (EntityRows.Count() > 0) {
                    var First = EntityRows.First();
                    using (IKernel Kernel = new StandardKernel(new Configure()))
                    {
                        EntityBase Entity = (EntityBase)Kernel.Get(((EntityType)First.EntityTypeID).Type);
                        Entity.LoadUpEntityFromView(Rows.Where(r => r.EntityID == EntityID));
                        this.AddEntity(Entity);
                    }
                }
            }
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
        protected void LoadByID(int ItemID)
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

        public static List<Item> GetItemsByID(IEnumerable<int> ItemIDs)
        {
            using (IKernel Kernel = new StandardKernel(new Configure()))
            {
                IPersistenceService PersistenceService = Kernel.Get<IPersistenceService>();
                var ViewResult = PersistenceService.RetreiveCompleteItems(ItemIDs);
                return GetItemsFromView(ViewResult);
            }
        }

        public static List<Item> GetItemsFromView(IEnumerable<Db.CompleteItem> ViewResult)
        {
            var ItemIDs = ViewResult.Select(v => v.ItemID).Distinct();
            List<Item> Output = new List<Item>();
            foreach (var ItemID in ItemIDs)
            {
                Output.Add(Item.GetItemFromView(ItemID, ViewResult));
            }
            return Output;
        }

        public static Item GetItemFromView(int ItemID, IEnumerable<Db.CompleteItem> ViewResult)
        {
            if (ViewResult == null)
            {
                throw new ArgumentNullException();
            }
            var ItemRows = ViewResult.Where(v => v.ItemID == ItemID);

            if (ItemRows.Count() > 0) {
                var First = ItemRows.First();
                using (IKernel Kernel = new StandardKernel(new Configure()))
                {
                    Item Item = (Item)Kernel.Get(((ItemType)First.ItemTypeID).Type);
                    Item.LoadByView(ItemID, ItemRows);
                    return Item;
                }
            }
            else
            {
                return null;
            }
        }

        internal void AddEntity(EntityBase Entity)
        {
            if (Entity == null)
            {
                throw new ArgumentNullException();
            }

            _AllEntities.Add(Entity);
            Entity.Item = this;
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
