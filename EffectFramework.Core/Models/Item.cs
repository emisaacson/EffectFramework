using System;
using System.Linq;
using System.Collections.Generic;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Services;

namespace EffectFramework.Core.Models
{
    public abstract class Item
    {
        public IEnumerable<EntityBase> AllEntities { get; protected set; }
        private Dictionary<EntityType, IEnumerable<EntityBase>> AllEntitiesByType;
        public int? ItemID { get; protected set; }
        public Guid Guid { get; protected set; }
        public bool Dirty { get; protected set; }
        public abstract ItemType Type { get; }

        protected readonly IPersistenceService PersistenceService;

        public SortedDictionary<DateTime, ItemRecord> ItemRecords { get; protected set; }
        public ItemRecord EffectiveRecord
        {
            get
            {
                return GetEffectiveRecordForDate(EffectiveDate);
            }
        }

        protected ItemRecord GetEffectiveRecordForDate(DateTime EffectiveDate)
        {
            var _EffectiveRecord = ItemRecords
                .Where(e =>
                    e.Key <= EffectiveDate &&
                    (!e.Value.EndEffectiveDate.HasValue || e.Value.EndEffectiveDate > EffectiveDate))
                .FirstOrDefault();

            if (!_EffectiveRecord.Equals(default(KeyValuePair<DateTime, ItemRecord>)) && _EffectiveRecord.Value != null)
            {
                return _EffectiveRecord.Value;
            }

            return null;
        }

        protected DateTime _EffectiveDate = DateTime.Now;
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

        public Item(IPersistenceService PersistenceService)
        {
            this.Dirty = true;
            this.PersistenceService = PersistenceService;
        }

        public Item(int ItemID, IPersistenceService PersistenceService, bool LoadItem = true)
        {
            this.ItemID = ItemID;
            this.PersistenceService = PersistenceService;
            if (LoadItem)
            {
                this.LoadByID(ItemID);
            }
        }

        public void Load()
        {
            if (!ItemID.HasValue)
            {
                throw new InvalidOperationException("Cannot reload an item with a null ID.");
            }
            LoadByID(ItemID.Value);
        }

        public void LoadByID(int ItemID)
        {
            if (this.ItemID.HasValue && ItemID != this.ItemID.Value)
            {
                throw new InvalidOperationException("Please do not reuse the same item object for an Item with a different ID.");
            }
            this.ItemID = ItemID;
            this.Dirty = false;
            this.Guid = PersistenceService.RetreiveGuidForItemRecord(this);
            var ItemRecordsList = PersistenceService.RetreiveAllItemRecords(this);

            ItemRecords = new SortedDictionary<DateTime, ItemRecord>();

            foreach (var ItemRecord in ItemRecordsList)
            {
                ItemRecords[ItemRecord.EffectiveDate] = ItemRecord;
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

        public ItemRecord GetOrCreateEffectiveDateRange(DateTime EffectiveDate)
        {
            if (EffectiveDate == null || EffectiveDate == default(DateTime))
            {
                throw new ArgumentNullException();
            }

            if (ItemRecords.ContainsKey(EffectiveDate))
            {
                return ItemRecords[EffectiveDate];
            }

            // If adding a new item record after an existing one, we copy the entities from the previous
            // and adjust the effective dates of all three to make a continuous timeline.
            if (ItemRecords.Count > 0 && ItemRecords.First().Key /*The oldest date*/ < EffectiveDate)
            {
                ItemRecord PreviousItemRecord = GetEffectiveRecordForDate(EffectiveDate);
                var NextItemRecord_kv = ItemRecords.Where(er => er.Key > PreviousItemRecord.EffectiveDate).FirstOrDefault();
                ItemRecord NextItemRecord = null;
                if (!NextItemRecord_kv.Equals(default(KeyValuePair<DateTime, ItemRecord>)))
                {
                    NextItemRecord = NextItemRecord_kv.Value;
                }
                ItemRecord NewItemRecord = new ItemRecord(PersistenceService);
                PreviousItemRecord.SetEndEffectiveDate(EffectiveDate);
                NewItemRecord.SetEffectiveDate(EffectiveDate);
                if (NextItemRecord != null)
                {
                    NewItemRecord.SetEndEffectiveDate(NextItemRecord.EffectiveDate);
                }
                NewItemRecord.CopyEntitiesFrom(PreviousItemRecord);
                ItemRecords[EffectiveDate] = NewItemRecord;
                return NewItemRecord;
            }
            // If adding a new item record before any existing, do not copy any entities, just
            // create a blank one
            else
            {
                ItemRecord NewItemRecord = new ItemRecord(PersistenceService);
                ItemRecord FirstItemRecord = null;
                if (ItemRecords.Count > 0)
                {
                    FirstItemRecord = ItemRecords.First().Value;
                }
                NewItemRecord.SetEffectiveDate(EffectiveDate);
                NewItemRecord.SetEndEffectiveDate(FirstItemRecord.EffectiveDate);
                ItemRecords[EffectiveDate] = NewItemRecord;
                return NewItemRecord;
            }
        }
    }
}
