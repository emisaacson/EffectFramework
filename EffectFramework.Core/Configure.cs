using System;
using System.Reflection;
using EffectFramework.Core.Models;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Models.Fields;
using EffectFramework.Core.Services;
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
        public static string PersistenceConnectionString;
        public static string CacheConnectionString;
        private static Type PersistenceServiceType = typeof(EntityFrameworkPersistenceService);
        private static Type CacheServiceType = typeof(NullCacheService);
        private static Type LoggingProviderType = typeof(NullLoggingProvider);

        private static IPersistenceService _PersistenceService;
        private static ICacheService _CacheService;

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

            Kernel.Bind<IPersistenceService>()
                .To(PersistenceServiceType)
                .InSingletonScope()
                .WithConstructorArgument("ConnectionString", PersistenceConnectionString);

            Kernel.Bind<ILoggingProvider>()
                .To(LoggingProviderType);

            Kernel.Bind<ICacheService>()
                .To(CacheServiceType)
                .InSingletonScope()
                .WithPropertyValue("ConnectionString", CacheConnectionString);
        }

        /// <summary>
        /// Registers the persistence service. The type should be wired up in the startup routine
        /// of the app. By default, the Entity Framework 7 persistence service is used.
        /// </summary>
        /// <typeparam name="PersistenceServiceT">The type of the persistence service (must implement IPersistenceService).</typeparam>
        public static void RegisterPersistenceService<PersistenceServiceT>()
            where PersistenceServiceT : IPersistenceService
        {
            PersistenceServiceType = typeof(PersistenceServiceT);
        }

        /// <summary>
        /// Registers the cache service. The type should be wired up in the startup routine of the app.
        /// By default, the Null cache service is used.
        /// </summary>
        /// <typeparam name="CacheServiceT">The type of the Cache Service (must implement ICacheService)</typeparam>
        public static void RegisterCacheService<CacheServiceT>()
            where CacheServiceT : ICacheService
        {
            CacheServiceType = typeof(CacheServiceT);
        }

        /// <summary>
        /// Registers the logging provider. The type should be wired up in the startup routine
        /// of the app. By default, the Null logging provider is used.
        /// </summary>
        /// <typeparam name="LoggingProviderT">The type of the Logging provider (must implement IPersistenceService).</typeparam>
        public static void RegisterLoggingProvider<LoggingProviderT>()
            where LoggingProviderT : ILoggingProvider
        {
            LoggingProviderType = typeof(LoggingProviderT);
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

        public static IPersistenceService GetPersistenceService()
        {
            if (_PersistenceService == null)
            {
                using (IKernel Kernel = new StandardKernel(new Configure()))
                {
                    _PersistenceService = Kernel.Get<IPersistenceService>();
                }
            }
            return _PersistenceService;
        }

        public static ICacheService GetCacheService()
        {
            if (_CacheService == null)
            {
                using (IKernel Kernel = new StandardKernel(new Configure()))
                {
                    _CacheService = Kernel.Get<ICacheService>();
                }
            }
            return _CacheService;
        }
    }
}
