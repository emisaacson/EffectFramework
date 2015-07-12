using EffectFramework.Core.Models.Db;
using EffectFramework.Core.Services;
using EffectFramework.Core.Exceptions;
using System;

namespace EffectFramework.Core.Models.Fields
{
    /// <summary>
    /// A class representing a single ID/value pair from the lookup
    /// table of the persistence store.
    /// </summary>
    [Serializable]
    public class LookupEntry
    {
        [NonSerialized]
        private Logger _Log;
        private Logger Log
        {
            get
            {
                if (_Log == null)
                {
                    _Log = new Logger(nameof(LookupEntry));
                }
                return _Log;
            }
        }

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
        public long? ID { get; private set; }
        private string _Value;
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (value != _Value)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ValidationFailedException("Cannot set an empty value for a Lookup Entry.");
                    }
                    this.Dirty = true;
                    this._Value = value;
                }
            }
        }
        public long TenantID { get; private set; }
        public Guid Guid { get; private set; }
        public bool Dirty { get; private set; }

        public bool FlagForDeletion { get; private set; } = false;
        public LookupCollection LookupCollection { get; private set; }

        public LookupEntry(LookupCollection LookupCollection)
        {
            if (LookupCollection == null)
            {
                Log.Error("Lookup collection is null.");
                throw new ArgumentNullException(nameof(LookupCollection));
            }
            this.Dirty = true;
            this.TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
            this.LookupCollection = LookupCollection;
        }

        public LookupEntry(long ID, string Value, long TenantID, Guid Guid, LookupCollection LookupCollection)
        {
            long _TenantID = Configure.GetTenantResolutionProvider().GetTenantID();

            if (_TenantID != TenantID)
            {
                Log.Fatal("Tenant ID does not match. Lookup Tenant ID: {0}, Global Tenant ID: {1}", TenantID, _TenantID);
                throw new Exceptions.FatalException("Data error.");
            }
            this.ID = ID;
            this.Value = Value;
            this.TenantID = TenantID;
            this.Guid = Guid;
            this.LookupCollection = LookupCollection;
            this.Dirty = false;
        }

        public void Delete()
        {
            this.Dirty = true;
            this.FlagForDeletion = true;
        }

        public bool PersistToDatabase(IDbContext ctx = null)
        {

            if (!PerformSanityCheck())
            {
                Log.Fatal("TenantID mismatch. Global TenantID: {0}, LookupEntry TenantID: {1}",
                    Configure.GetTenantResolutionProvider().GetTenantID(), this.TenantID);
                throw new Exceptions.FatalException("Invalid data exception.");
            }

            ObjectIdentity Identity = null;
            if (this.FlagForDeletion)
            {
                PersistenceService.SaveAndDeleteLookupEntry(this, ctx);
            }
            else
            {
                Identity = PersistenceService.SaveSingleLookupEntry(this, ctx);
            }
            if (Identity != null)
            {
                this.ID = Identity.ObjectID;
                this.Guid = Identity.ObjectGuid;
            }
            else
            {
                this.ID = null;
                this.Guid = default(Guid);
            }

            this.Dirty = false;

            return Identity == null ? false : Identity.DidUpdate;
        }

        public bool PerformSanityCheck()
        {
            long _TenantID = Configure.GetTenantResolutionProvider().GetTenantID();

            if (_TenantID != this.TenantID)
            {
                return false;
            }

            return true;
        }
    }
}
