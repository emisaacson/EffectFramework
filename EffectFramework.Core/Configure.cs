﻿using System;
using System.Reflection;
using EffectFramework.Core.Models;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Models.Fields;
using EffectFramework.Core.Services;
using Ninject.Modules;

namespace EffectFramework.Core
{
    public class Configure : NinjectModule
    {
        public static string ConnectionString;
        private static Type PersistenceServiceType = typeof(EntityFrameworkPersistenceService);
        private static Type LoggingProvider = typeof(NullLoggingProvider);

        public Configure()
        {

        }
        public Configure(string ConnectionString)
        {
            Configure.ConnectionString = ConnectionString;
        }
        public override void Load()
        {
            if (ConnectionString == null)
            {
                throw new InvalidOperationException("Must set connection string.");
            }
            Kernel.Bind<IPersistenceService>()
                    .To(PersistenceServiceType)
                    .WithConstructorArgument("ConnectionString", ConnectionString);

            Kernel.Bind<ILoggingProvider>()
                    .To(LoggingProvider);
        }

        public static void RegisterPersistenceService<PersistenceServiceT>()
            where PersistenceServiceT : IPersistenceService
        {
            PersistenceServiceType = typeof(PersistenceServiceT);
        }

        public static void RegisterLoggingProvider<LoggingProviderT>()
            where LoggingProviderT : ILoggingProvider
        {
            LoggingProvider = typeof(LoggingProviderT);
        }

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
    }
}
