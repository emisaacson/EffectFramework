using EffectFramework.Core.Exceptions;
using EffectFramework.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models.Fields
{
    [Serializable]
    public class LookupCollection
    {
        [NonSerialized]
        protected Logger _Log;
        protected Logger Log {
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
        public string Name { get; private set; }
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

        public LookupCollection(int LookupID)
        {
            this.TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
        }

        public bool PersistToDatabase(Db.IDbContext ctx = null)
        {
            if (!PerformSanityCheck())
            {
                Log.Fatal("TenantID mismatch. Global TenantID: {0}, Lookup TenantID: {1}",
                    Configure.GetTenantResolutionProvider().GetTenantID(), this.TenantID);
                throw new FatalException("Invalid data exception.");
            }

            throw new NotImplementedException();
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
    }
}
