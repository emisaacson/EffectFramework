using System;
using EffectFramework.Core.Services;
using Ninject;

namespace EffectFramework.Core.Models.Entities
{
    public static class EntityFactory
    {
        public static EntityT GenerateEntityFromDbObject<EntityT>(Db.Entity Entity) where EntityT : EntityBase, new()
        {
            EntityT Instance = new EntityT();
            Instance.LoadUpEntity(Entity);
            return Instance;
        }

        public static EntityBase GenerateEntityFromDbObject(Db.Entity Entity)
        {
            if (Entity == null)
            {
                throw new ArgumentNullException();
            }

            int EntityTypeID = Entity.EntityTypeID;
            EntityType EntityType = (EntityType)EntityTypeID;

            if (!typeof(EntityBase).IsAssignableFrom(EntityType.Type))
            {
                throw new InvalidOperationException("Cannot create entity from this type.");
            }

            EntityBase Output = (EntityBase)Activator.CreateInstance(EntityType.Type);

            using (IKernel Kernel = new StandardKernel(new Configure()))
            {
                IPersistenceService PersistenceService = Kernel.Get<IPersistenceService>();
                Output.PersistenceService = PersistenceService;
            }

            Output.LoadUpEntity(Entity);

            return Output;
        }
    }
}
