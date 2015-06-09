﻿using System;
using System.Linq;
using System.Collections.Generic;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Services;
using Ninject;
using Ninject.Parameters;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// All items must inherit this base class.
    /// </summary>
    [Serializable]
    public abstract class Item
    {
        [NonSerialized]
        protected Logger _Log;
        protected Logger Log
        {
            get
            {
                if (_Log == null)
                {
                    _Log = new Logger(GetType().Name);
                }
                return _Log;
            }
        }

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

        [NonSerialized]
        protected IPersistenceService _PersistenceService;
        protected IPersistenceService PersistenceService
        {
            get
            {
                if (_PersistenceService == null)
                {
                    _PersistenceService = Configure.GetPersistenceService();
                }
                return _PersistenceService;
            }
        }
        [NonSerialized]
        protected ICacheService _CacheService;
        protected ICacheService CacheService
        {
            get
            {
                if (_CacheService == null)
                {
                    _CacheService = Configure.GetCacheService();
                }
                return _CacheService;
            }
        }

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
        /// <param name="Sparse">If set to <c>true</c>, only load entities for the current EffectiveRecord.</param>
        public Item(bool Sparse = false)
        {
            this.Dirty = true;
            this.Sparse = Sparse;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="ItemID">The item identifier.</param>
        /// <param name="PersistenceService">The persistence service.</param>
        /// <param name="LoadItem">If set to <c>true</c>, retreive all data for this item from the PersistenceService.</param>
        /// <param name="Sparse">If set to <c>true</c>, only load entities for the current EffectiveRecord.</param>
        public Item(int ItemID, bool LoadItem = true, bool Sparse = false)
        {
            this.ItemID = ItemID;
            this.Sparse = Sparse;

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
            return new EntityCollection(this, EffectiveDate);
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

                    EntityBase Entity = EntityBase.GetEntityByType((EntityType)First.EntityTypeID, this);
                    Entity.LoadUpEntityFromView(Rows.Where(r => r.EntityID == EntityID));
                    this.AddEntity(Entity);
                }
            }
            this.Dirty = false;
        }


        /// <summary>
        /// Persist the item and all of its entities to database.
        /// </summary>
        /// <param name="ctx">The database context. If one is not provided, a new one will be created with a transaction.</param>
        /// <returns>True if the Item or any of its entites were changed.</returns>
        public bool PersistToDatabase(Db.IDbContext ctx = null)
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

                List<bool> Results = new List<bool>();
                foreach (var Entity in _AllEntities)
                {
                    Results.Add(Entity.PersistToDatabase(db));
                }
                bool ThisDidChange = Identity.DidUpdate || Results.Any(r => r == true);

                RemoveDeadEntities();

                this.Dirty = false;

                if (ctx == null)
                {
                    db.Commit();
                }

                if (ThisDidChange)
                {
                    CacheService.DeleteObject(string.Format("Item:{0}", ItemID));
                }

                return ThisDidChange;
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

            this.Dirty = false;
        }

        public static Item GetItemByID(int ItemID)
        {
            IPersistenceService PersistenceService = Configure.GetPersistenceService();
            var ViewResult = PersistenceService.RetreiveCompleteItems(new int [] { ItemID });
            var Items = GetItemsFromView(ViewResult);
            return Items.FirstOrDefault();
        }

        public static List<Item> GetItemsByID(IEnumerable<int> ItemIDs)
        {

            ICacheService CacheService = Configure.GetCacheService();
            IPersistenceService PersistenceService = Configure.GetPersistenceService();

            List<int> MissingItemIDs = new List<int>();
            List<Item> CachedItems = new List<Item>();
            List<Item> NotCachedItems = new List<Item>();
            foreach (var ItemID in ItemIDs)
            {
                string Key = string.Format("Item:{0}", ItemID);
                var MaybeCompleteItem = (Item)CacheService.GetObject(Key);

                if (MaybeCompleteItem != null)
                {
                    CachedItems.Add(MaybeCompleteItem);
                }
                else
                {
                    MissingItemIDs.Add(ItemID);
                }
            }

            if (MissingItemIDs.Count() > 0)
            {
                var ViewResult = PersistenceService.RetreiveCompleteItems(MissingItemIDs);
                NotCachedItems = GetItemsFromView(ViewResult);
                foreach (Item Item in NotCachedItems)
                {
                    string Key = string.Format("Item:{0}", Item.ItemID);
                    CacheService.StoreObject(Key, Item);
                }
            }

            CachedItems.AddRange(NotCachedItems);
            return CachedItems;
        }

        public static List<Item> GetItemsFromView(IEnumerable<Db.CompleteItem> ViewResult)
        {

            ICacheService CacheService = Configure.GetCacheService();
            IPersistenceService PersistenceService = Configure.GetPersistenceService();

            var ItemIDs = ViewResult.Select(v => v.ItemID).Distinct();

            List<int> MissingItemIDs = new List<int>();
            List<Item> CachedItems = new List<Item>();
            List<Item> NotCachedItems = new List<Item>();
            foreach (var ItemID in ItemIDs)
            {
                string Key = string.Format("Item:{0}", ItemID);
                var MaybeCompleteItem = (Item)CacheService.GetObject(Key);

                if (MaybeCompleteItem != null)
                {
                    CachedItems.Add(MaybeCompleteItem);
                }
                else
                {
                    MissingItemIDs.Add(ItemID);
                }
            }

            if (MissingItemIDs.Count() > 0)
            {
                foreach (int ItemID in MissingItemIDs)
                {
                    var Item = GetItemFromView(ItemID, ViewResult);
                    NotCachedItems.Add(Item);
                    string Key = string.Format("Item:{0}", ItemID);
                    CacheService.StoreObject(Key, Item);
                }
            }

            CachedItems.AddRange(NotCachedItems);
            return CachedItems;
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
                Item Item = (Item)Activator.CreateInstance(((ItemType)First.ItemTypeID).Type);
                Item.LoadByView(ItemID, ItemRows);
                return Item;
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
