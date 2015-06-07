using System;
using EffectFramework.Core.Services;
using Ninject;

namespace EffectFramework.Core.Models.Entities
{
    /// <summary>
    /// Static class to create an Entity object from a raw database object.
    /// </summary>
    public static class EntityFactory
    {
        /// <summary>
        /// Generates an entity from database object.
        /// </summary>
        /// <typeparam name="EntityT">The type of the Entity.</typeparam>
        /// <param name="Entity">The entity.</param>
        /// <returns>The generated Entity object.</returns>
        public static EntityT GenerateEntityFromDbObject<EntityT>(Db.Entity Entity) where EntityT : EntityBase, new()
        {
            EntityT Instance = new EntityT();
            Instance.LoadUpEntity(Entity);
            return Instance;
        }

        /// <summary>
        /// Generates an entity from database object.
        /// </summary>
        /// <param name="Entity">The entity.</param>
        /// <returns>The generated Entity object.</returns>
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

            Output.PersistenceService = Configure.GetPersistenceService();

            Output.LoadUpEntity(Entity);

            return Output;
        }
    }
}
