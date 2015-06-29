﻿using EffectFramework.Core.Exceptions;
using EffectFramework.Core.Models.Db;
using EffectFramework.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EffectFramework.Core.Models.Fields
{
    [Serializable]
    public class LookupCollection
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
                if (value != _Name)
                {
                    Dirty = true;
                    _Name = value;
                }
            
            }
        }
        public string OriginalName { get; private set; }
        public int? LookupTypeID { get; private set; }

        public bool FlagForDeletion { get; private set; } = false;

        private List<LookupEntry> _Choices;
        public IReadOnlyCollection<LookupEntry> Choices
        {
            get
            {
                if (_Choices == null)
                {
                    if (this.LookupTypeID.HasValue)
                    {
                        _Choices = PersistenceService.GetLookupEntries(this.LookupTypeID.Value, this).ToList();
                    }
                    else
                    {
                        _Choices = new List<LookupEntry>();
                    }
                }
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
        }

        public LookupCollection(int LookupCollectionID)
        {
            this.TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
            LoadById(LookupCollectionID);
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

            RefreshOriginalValues();
        }

        private void LoadById(int LookupCollectionID)
        {
            bool ShouldReplaceCache = false;
            LookupCollection LookupFromDatabase = (LookupCollection)CacheService.GetObject(string.Format("LookupCollection:{0}", LookupCollectionID));
            if (LookupFromDatabase == null)
            {
                LookupFromDatabase = PersistenceService.GetLookupCollectionById(LookupCollectionID);
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

            RefreshOriginalValues();

            if (ShouldReplaceCache)
            {
                CacheService.StoreObject(string.Format("LookupCollection:{0}", LookupCollectionID), LookupFromDatabase);
            }
        }

        public bool PersistToDatabase(Db.IDbContext ctx = null)
        {
            if (!PerformSanityCheck())
            {
                Log.Fatal("TenantID mismatch. Global TenantID: {0}, Lookup TenantID: {1}",
                    Configure.GetTenantResolutionProvider().GetTenantID(), this.TenantID);
                throw new FatalException("Invalid data exception.");
            }

            ObjectIdentity Identity = null;
            if (this.FlagForDeletion)
            {
                PersistenceService.SaveAndDeleteLookupCollection(this, ctx);
            }
            else
            {
                Identity = PersistenceService.SaveLookupCollection(this, ctx);
            }

            if (Identity != null)
            {
                this.LookupTypeID = Identity.ObjectID;
                this.Guid = Identity.ObjectGuid;
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
                    Choice.PersistToDatabase(ctx);
                }
            }
            
            foreach (var Choice in _Choices.Where(c => c.FlagForDeletion))
            {
                _Choices.Remove(Choice);
            }

            this.Dirty = false;

            if (Identity == null || Identity.DidUpdate)
            {
                CacheService.DeleteObject(string.Format("LookupCollection:{0}", LookupTypeID));
            }

            return Identity == null ? false : Identity.DidUpdate;
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
            if (Value == null)
            {
                Log.Error("Lookup entry value is null. LookupCollectionID: {0}", this.LookupTypeID);
                throw new ArgumentNullException(nameof(Value));
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

        public static IEnumerable<LookupCollection> GetAllLookupCollections()
        {
            return Configure.GetPersistenceService().GetAllLookupCollections();
        }
    }
}
