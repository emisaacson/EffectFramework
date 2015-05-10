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
        public override void Load()
        {
            Kernel.Bind<IPersistenceService>()
                    .To<EntityFrameworkPersistenceService>();
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
