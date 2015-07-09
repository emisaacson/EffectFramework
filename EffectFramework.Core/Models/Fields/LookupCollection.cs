using EffectFramework.Core.Exceptions;
using EffectFramework.Core.Models.Db;
using EffectFramework.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EffectFramework.Core.Models.Fields
{
    [Serializable]
    public class LookupCollection : ICacheable
    {
        [NonSerialized]
        private Logger _Log;
        private Logger Log {
            get
            {
                if (_Log == null)
                {
                    _Log = new Logger(nameof(LookupCollection));
                }
                return _Log;
            }
        }
        public bool Dirty { get; private set; }
        public Guid Guid { get; private set; }
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value == null)
                {
                    throw new ValidationFailedException("Lookup name cannot be empty.");
                }
                if (value != _Name)
                {
                    Dirty = true;
                    _Name = value;
                }
            
            }
        }
        public string OriginalName { get; private set; }
        public int? LookupTypeID { get; private set; }
        public bool IsReadOnly { get; private set; }

        public bool FlagForDeletion { get; private set; } = false;

        private List<LookupEntry> _Choices;
        public IReadOnlyCollection<LookupEntry> Choices
        {
            get
            {
                RefreshChoices();
                return _Choices.AsReadOnly();
            }
        }
        public int TenantID { get; private set; }

        [NonSerialized]
        private IPersistenceService _PersistenceService;
        private IPersistenceService PersistenceService
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
        private ICacheService _CacheService;
        private ICacheService CacheService
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

        public LookupCollection()
        {
            this.Dirty = true;
            this.TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
            RefreshChoices();
        }

        public LookupCollection(int LookupCollectionID, IDbContext ctx = null)
        {
            this.TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
            LoadById(LookupCollectionID, ctx);
        }

        public LookupCollection(Db.LookupType DbLookupType)
        {
            this.TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
            if (this.TenantID != DbLookupType.TenantID)
            {
                Log.Fatal("Tenant ID does not match. Global Tenant ID: {0}, Db Lookup Type: {1}",
                    TenantID, DbLookupType.TenantID);
                throw new FatalException("Data error.");
            }

            this.Name = DbLookupType.Name;
            this.LookupTypeID = DbLookupType.LookupTypeID;
            this.Guid = DbLookupType.Guid;
            this.Dirty = false;
            this.IsReadOnly = DbLookupType.IsReadOnly;
            RefreshChoices();

            RefreshOriginalValues();
        }

        private void LoadById(int LookupCollectionID, IDbContext ctx = null)
        {
            bool ShouldReplaceCache = false;
            LookupCollection LookupFromDatabase = (LookupCollection)CacheService.GetObject(CacheUtility.GetCacheString<LookupCollection>(LookupCollectionID));
            if (LookupFromDatabase == null)
            {
                LookupFromDatabase = PersistenceService.GetLookupCollectionById(LookupCollectionID, ctx);
                ShouldReplaceCache = true;
            }

            this.TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
            if (this.TenantID != LookupFromDatabase.TenantID)
            {
                Log.Fatal("Tenant ID does not match. Global Tenant ID: {0}, Other Lookup Type: {1}",
                    TenantID, LookupFromDatabase.TenantID);
                throw new FatalException("Data error.");
            }

            this.Name = LookupFromDatabase.Name;
            this.LookupTypeID = LookupFromDatabase.LookupTypeID;
            this.Guid = LookupFromDatabase.Guid;
            this.Dirty = false;
            this.IsReadOnly = LookupFromDatabase.IsReadOnly;
            RefreshChoices(ctx);

            RefreshOriginalValues();

            if (ShouldReplaceCache)
            {
                CacheService.StoreObject(GetCacheKey(), this);
            }
        }

        public bool PersistToDatabase(Db.IDbContext ctx = null)
        {
            if (IsReadOnly)
            {
                Log.Error("Cannot save read only collection.");
                throw new FatalException("Cannot save read only collection.");
            }
            if (!PerformSanityCheck())
            {
                Log.Fatal("TenantID mismatch. Global TenantID: {0}, Lookup TenantID: {1}",
                    Configure.GetTenantResolutionProvider().GetTenantID(), this.TenantID);
                throw new FatalException("Invalid data exception.");
            }

            IDbContext db;
            if (ctx == null)
            {
                db = Configure.GetPersistenceService().GetDbContext();
                db.BeginTransaction();
            }
            else
            {
                db = ctx;
            }

            ObjectIdentity Identity = null;
            bool DidUpdate = false;
            if (this.FlagForDeletion)
            {
                PersistenceService.SaveAndDeleteLookupCollection(this, db);
            }
            else
            {
                Identity = PersistenceService.SaveLookupCollection(this, db);
            }

            if (Identity != null)
            {
                this.LookupTypeID = Identity.ObjectID;
                this.Guid = Identity.ObjectGuid;
                DidUpdate = Identity.DidUpdate;
            }
            else
            {
                this.LookupTypeID = null;
                this.Guid = default(Guid);
            }

            foreach (var Choice in Choices)
            {
                if (Choice.Dirty)
                {
                    if (Choice.PersistToDatabase(db))
                    {
                        DidUpdate = true;
                    }
                }
            }
            
            _Choices.RemoveAll(c => c.FlagForDeletion);

            if (ctx == null)
            {
                db.Commit();
            }

            this.Dirty = false;

            if (Identity == null || DidUpdate)
            {
                CacheService.DeleteObject(CacheUtility.GetCacheString<LookupCollection>(LookupTypeID));
            }

            return Identity == null ? false : DidUpdate;
        }

        public bool PerformSanityCheck()
        {
            int _TenantID = Configure.GetTenantResolutionProvider().GetTenantID();

            if (_TenantID != this.TenantID)
            {
                return false;
            }

            return true;
        }

        public void AddLookupEntry(string Value)
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                Log.Error("Lookup entry value is null. LookupCollectionID: {0}", this.LookupTypeID);
                throw new ValidationFailedException("Lookup value cannot be empty.");
            }

            LookupEntry LookupEntry = new LookupEntry(this);
            LookupEntry.Value = Value;

            this._Choices.Add(LookupEntry);
        }

        private void RefreshOriginalValues()
        {
            if (!this.Dirty) {
                this.OriginalName = this.Name;
            }
        }

        public void Delete()
        {
            this.Dirty = true;
            this.FlagForDeletion = true;
        }

        private void RefreshChoices(IDbContext ctx = null)
        {
            if (_Choices == null)
            {
                if (this.LookupTypeID.HasValue)
                {
                    _Choices = PersistenceService.GetLookupEntries(this.LookupTypeID.Value, this, ctx).ToList();
                }
                else
                {
                    _Choices = new List<LookupEntry>();
                }
            }
        }

        public static IEnumerable<LookupCollection> GetAllLookupCollections(IDbContext ctx = null)
        {
            return Configure.GetPersistenceService().GetAllLookupCollections(ctx);
        }

        public string GetCacheKey()
        {
            if (!this.LookupTypeID.HasValue)
            {
                throw new InvalidOperationException("Cannot get a cache key for an unpersisted LookupCollection.");
            }

            return string.Format(CacheKeyFormatString, this.LookupTypeID.Value);
        }

        public const string CacheKeyFormatString = "LookupCollection:{0}";
    }
}
