using System;
using System.Reflection;
using EffectFramework.Core.Models;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Models.Fields;
using EffectFramework.Core.Services;
using System.Threading;
using Ninject.Modules;
using Ninject;

namespace EffectFramework.Core
{
    public class Configure : NinjectModule
    {
        /// <summary>
        /// Gets or sets the global connection string used for all instances of the 
        /// persistence service. This should be set in the startup routine of the app.
        /// </summary>
        private static string _PersistenceConnectionString;
        public static string PersistenceConnectionString
        {
            get
            {
                try
                {
                    ConnectionLock.EnterReadLock();
                    return _PersistenceConnectionString;
                }
                finally
                {
                    try
                    {
                        ConnectionLock.ExitReadLock();
                    }
                    catch (Exception e)
                    {
                        Log.Error("Hell froze over.", e);
                    }
                }
            }
            set
            {
                try
                {
                    ConnectionLock.EnterWriteLock();
                    _PersistenceConnectionString = value;
                }
                finally
                {
                    try
                    {
                        ConnectionLock.ExitWriteLock();
                    }
                    catch (Exception e)
                    {
                        Log.Error("Hell froze over.", e);
                    }
                }
            }
        }
        private static string _CacheConnectionString;
        public static string CacheConnectionString
        {
            get
            {
                try
                {
                    ConnectionLock.EnterReadLock();
                    return _CacheConnectionString;
                }
                finally
                {
                    try
                    {
                        ConnectionLock.ExitReadLock();
                    }
                    catch (Exception e)
                    {
                        Log.Error("Hell froze over.", e);
                    }
                }
            }
            set
            {
                try
                {
                    ConnectionLock.EnterWriteLock();
                    _CacheConnectionString = value;
                }
                finally
                {
                    try
                    {
                        ConnectionLock.ExitWriteLock();
                    }
                    catch (Exception e)
                    {
                        Log.Error("Hell froze over.", e);
                    }
                }
            }
        }
        private static ReaderWriterLockSlim ConnectionLock = new ReaderWriterLockSlim();

        private static Type PersistenceServiceType = typeof(EntityFrameworkPersistenceService);
        private static Type CacheServiceType = typeof(NullCacheService);
        private static Type LoggingProviderType = typeof(NullLoggingProvider);
        private static Type ObjectQueryProviderType = typeof(NullObjectQuery);
        private static Type TenantResolutionProviderType = typeof(DefaultTenantResolver);
        private static ReaderWriterLockSlim TypeLock = new ReaderWriterLockSlim();

        private static IPersistenceService _PersistenceService;
        private static object PersistenceLock = new object();
        private static ICacheService _CacheService;
        private static object CacheLock = new object();
        private static ITenantResolutionProvider _TenantResolutionProvider;
        private static object TenantLock = new object();

        private static Logger Log = new Logger("Configure");
        public Configure()
        {

        }
        public Configure(string PersistenceConnectionString, string CacheConnectionString = null)
        {
            Configure.PersistenceConnectionString = PersistenceConnectionString;
            Configure.CacheConnectionString = CacheConnectionString;
        }
        public override void Load()
        {
            try {
                TypeLock.EnterReadLock();

                Kernel.Bind<IPersistenceService>()
                    .To(PersistenceServiceType)
                    .InSingletonScope()
                    .WithConstructorArgument("ConnectionString", PersistenceConnectionString);

                Kernel.Bind<ILoggingProvider>()
                    .To(LoggingProviderType);

                Kernel.Bind<IObjectQueryProvider>()
                    .To(ObjectQueryProviderType);

                Kernel.Bind<ICacheService>()
                    .To(CacheServiceType)
                    .InSingletonScope()
                    .WithPropertyValue("ConnectionString", CacheConnectionString);

                Kernel.Bind<ITenantResolutionProvider>()
                    .To(TenantResolutionProviderType);
            }
            finally
            {
                try
                {
                    TypeLock.ExitReadLock();
                }
                catch (Exception e)
                {
                    Log.Error("Hell froze over.", e);
                }
            }
        }

        /// <summary>
        /// Registers the persistence service. The type should be wired up in the startup routine
        /// of the app. By default, the Entity Framework 7 persistence service is used.
        /// </summary>
        /// <typeparam name="PersistenceServiceT">The type of the persistence service (must implement IPersistenceService).</typeparam>
        public static void RegisterPersistenceService<PersistenceServiceT>()
            where PersistenceServiceT : IPersistenceService
        {
            try
            {
                TypeLock.EnterWriteLock();
                PersistenceServiceType = typeof(PersistenceServiceT);
            }
            finally
            {
                try
                {
                    TypeLock.ExitWriteLock();
                }
                catch (Exception e)
                {
                    Log.Error("Hell froze over.", e);
                }
            }
        }

        /// <summary>
        /// Registers the cache service. The type should be wired up in the startup routine of the app.
        /// By default, the Null cache service is used.
        /// </summary>
        /// <typeparam name="CacheServiceT">The type of the Cache Service (must implement ICacheService)</typeparam>
        public static void RegisterCacheService<CacheServiceT>()
            where CacheServiceT : ICacheService
        {
            try
            {
                TypeLock.EnterWriteLock();
                CacheServiceType = typeof(CacheServiceT);
            }
            finally
            {
                try
                {
                    TypeLock.ExitWriteLock();
                }
                catch (Exception e)
                {
                    Log.Error("Hell froze over.", e);
                }
            }
        }

        /// <summary>
        /// Registers the logging provider. The type should be wired up in the startup routine
        /// of the app. By default, the Null logging provider is used.
        /// </summary>
        /// <typeparam name="LoggingProviderT">The type of the Logging provider (must implement IPersistenceService).</typeparam>
        public static void RegisterLoggingProvider<LoggingProviderT>()
            where LoggingProviderT : ILoggingProvider
        {
            try
            {
                TypeLock.EnterWriteLock();
                LoggingProviderType = typeof(LoggingProviderT);
            }
            finally
            {
                try
                {
                    TypeLock.ExitWriteLock();
                }
                catch (Exception e)
                {
                    Log.Error("Hell froze over.", e);
                }
            }
        }

        /// <summary>
        /// Registers the tenant resolution provider. The type should be wired up in the startup
        /// routine of the app. By default, the DefaultTenantResolver is used.
        /// </summary>
        /// <typeparam name="TenantResolutionProviderT">The type of the Tenant resolution provider (must implement ITenantResolutionProvider)</typeparam>
        public static void RegisterTenantResolutionProvider<TenantResolutionProviderT>()
            where TenantResolutionProviderT : ITenantResolutionProvider
        {
            try
            {
                TypeLock.EnterWriteLock();
                TenantResolutionProviderType = typeof(TenantResolutionProviderT);
            }
            finally
            {
                try
                {
                    TypeLock.ExitWriteLock();
                }
                catch (Exception e)
                {
                    Log.Error("Hell froze over.", e);
                }
            }
        }

        /// <summary>
        /// Registers the object query provider. The type should be wired up in the startup routine
        /// of the app. By default, the Null object query provider is used.
        /// </summary>
        /// <typeparam name="ObjectQueryProviderT">The type of the object query provider (must implement IObjectQueryProvider).</typeparam>
        public static void RegisterObjectQueryProvider<ObjectQueryProviderT>()
            where ObjectQueryProviderT : IObjectQueryProvider
        {
            try
            {
                TypeLock.EnterWriteLock();
                ObjectQueryProviderType = typeof(ObjectQueryProviderT);
            }
            finally
            {
                try
                {
                    TypeLock.ExitWriteLock();
                }
                catch (Exception e)
                {
                    Log.Error("Hell froze over.", e);
                }
            }
        }

        /// <summary>
        /// Each application will provide its own Item, Entity and Field types. These are declared on a 
        /// class as static fields and must be registered so the framework is aware of all the data types
        /// available. All three classes can be registered with this single call.
        /// </summary>
        /// <typeparam name="TItemType">The type of the Item type class.</typeparam>
        /// <typeparam name="TEntityType">The type of the Entity type class.</typeparam>
        /// <typeparam name="TFieldType">The type of the Field type class.</typeparam>
        public static void RegisterTypeClasses<TItemType, TEntityType, TFieldType>()
            where TItemType : ItemType
            where TEntityType : EntityType 
            where TFieldType : FieldType
        {
            try
            {
                TypeLock.EnterWriteLock();

                var ItemTypes = typeof(TItemType).GetFields(BindingFlags.Public | BindingFlags.Static);
                var EventTypes = typeof(TEntityType).GetFields(BindingFlags.Public | BindingFlags.Static);
                var FieldTypes = typeof(TFieldType).GetFields(BindingFlags.Public | BindingFlags.Static);
    
                foreach (var ItemType in ItemTypes)
                {
                    if (typeof(ItemType).IsAssignableFrom(ItemType.FieldType))
                    {
                        var RegisteredType = ItemType.GetValue(null);
                    }
                }
    
                foreach (var EventType in EventTypes)
                {
                    if (typeof(EntityType).IsAssignableFrom(EventType.FieldType))
                    {
                        var RegisteredType = EventType.GetValue(null);
                    }
                }
    
                foreach (var FieldType in FieldTypes)
                {
                    if (typeof(FieldType).IsAssignableFrom(FieldType.FieldType))
                    {
                        var RegisteredType = FieldType.GetValue(null);
                    }
                }
            }
            finally
            {
                try
                {
                    TypeLock.ExitWriteLock();
                }
                catch (Exception e)
                {
                    Log.Error("Hell froze over.", e);
                }
            }
        }

        public static IPersistenceService GetPersistenceService()
        {
            if (_PersistenceService == null)
            {
                lock (PersistenceLock)
                {
                    if (_PersistenceService == null)
                    {
                        using (IKernel Kernel = new StandardKernel(new Configure()))
                        {
                            _PersistenceService = Kernel.Get<IPersistenceService>();
                        }
                    }
                }
            }
            return _PersistenceService;
        }

        public static ICacheService GetCacheService()
        {
            if (_CacheService == null)
            {
                lock (CacheLock)
                {
                    if (_CacheService == null)
                    {
                        using (IKernel Kernel = new StandardKernel(new Configure()))
                        {
                            _CacheService = Kernel.Get<ICacheService>();
                        }
                    }
                }
            }
            return _CacheService;
        }

        public static IObjectQueryProvider GetObjectQueryProvider()
        {
            using (IKernel Kernel = new StandardKernel(new Configure()))
            {
                return Kernel.Get<IObjectQueryProvider>();
            }
        }

        public static ITenantResolutionProvider GetTenantResolutionProvider()
        {
            if (_TenantResolutionProvider == null)
            {
                lock (TenantLock)
                {
                    if (_TenantResolutionProvider == null)
                    {
                        using (IKernel Kernel = new StandardKernel(new Configure()))
                        {
                            _TenantResolutionProvider = Kernel.Get<ITenantResolutionProvider>();
                        }
                    }
                }
            }
            return _TenantResolutionProvider;
        }
    }
}
