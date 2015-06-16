using System;
using System.Collections.Generic;
using System.Linq;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Services;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// An EntityCollection contains all Entities for a particular Item that intersect
    /// with the supplied EffectiveDate. It can be retreived from the Item class via
    /// the EffectiveRecord property or the GetEntityCollectionForDate method.
    /// 
    /// It also has some useful methods for creating and querying entities that intersect
    /// with the given EffectiveDate.
    /// </summary>
    [Serializable]
    public class EntityCollection
    {

        /// <summary>
        /// Pulls relavent Entities from the bound Item object.
        /// </summary>
        /// <value>
        /// All entities intersecting with the object's EffectiveDate.
        /// </value>
        public IEnumerable<EntityBase> AllEntities
        {
            get
            {
                return Item.AllEntities
                    .Where(e =>
                        e.EffectiveDate <= this.EffectiveDate &&
                        (!e.EndEffectiveDate.HasValue || e.EndEffectiveDate > this.EffectiveDate)).AsEnumerable();
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

        /// <summary>
        /// Gets the effective date.
        /// </summary>
        /// <value>
        /// The effective date.
        /// </value>
        public DateTime EffectiveDate { get; private set; }

        /// <summary>
        /// The Item that created with EntityCollection.
        /// </summary>
        public Item Item { get; private set; }

        internal EntityCollection(Item Item, DateTime EffectiveDate)
        {
            this.Item = Item;
            this.EffectiveDate = EffectiveDate;
        }

        /// <summary>
        /// Retreives a single entity from the Entities intersecting with the current EffectiveDate,
        /// ordered by EffectiveDate ascending and plucking the first, or null if no such Entity
        /// exists.
        /// </summary>
        /// <typeparam name="EntityT">The type of the Entity (a child class of EntityBase).</typeparam>
        /// <returns>The Entity if found, or default (null) otherwise.</returns>
        public EntityT GetFirstEntityOrDefault<EntityT>() where EntityT : EntityBase
        {
            EntityT Instance = (EntityT)EntityBase.GetEntityBySystemType(typeof(EntityT));

            return (EntityT)GetFirstEntityOrDefault(Instance.Type);
        }

        /// <summary>
        /// Retreives all entities from the Entities intersecting with the current EffectiveDate,
        /// ordered by EffectiveDate ascending.
        /// </summary>
        /// <typeparam name="EntityT">The type of the Entity (a child class of EntityBase).</typeparam>
        /// <returns>An IEnumerable of all Entities found.</returns>
        public IEnumerable<EntityT> GetAllEntitiesOfType<EntityT>() where EntityT : EntityBase
        {
            EntityT Instance = (EntityT)EntityBase.GetEntityBySystemType(typeof(EntityT));

            return GetAllEntitiesOfType(Instance.Type).Cast<EntityT>();
        }


        /// <summary>
        /// Retreives a single entity from the Entities intersecting with the current EffectiveDate,
        /// ordered by EffectiveDate ascending and plucking the first, or null if no such Entity
        /// exists.
        /// </summary>
        /// <param name="EntityType">The type of the entity.</param>
        /// <returns>The Entity if found, or default (null) otherwise.</returns>
        public EntityBase GetFirstEntityOrDefault(EntityType EntityType)
        {
            return AllEntities.Where(e => e.Type == EntityType).OrderBy(e => e.EffectiveDate).FirstOrDefault();
        }

        /// <summary>
        /// Retreives all entities from the Entities intersecting with the current EffectiveDate,
        /// ordered by EffectiveDate ascending.
        /// </summary>
        /// <param name="EntityType">The type of the entity.</param>
        /// <returns>An IEnumerable of all Entities found.</returns>
        public IEnumerable<EntityBase> GetAllEntitiesOfType(EntityType EntityType)
        {
            return AllEntities.Where(e => e.Type == EntityType).OrderBy(e => e.EffectiveDate).AsEnumerable();
        }

        /// <summary>
        /// Creates an entity using the EntityCollection's EffectiveDate
        /// as start date and optionally an EndEffectiveDate if provided.
        /// </summary>
        /// <typeparam name="EntityT">The type of the Entity to create.</typeparam>
        /// <param name="EndEffectiveDate">An optional EndEffectiveDate to set.</param>
        /// <returns>The new Entity.</returns>
        public EntityT CreateEntity<EntityT>(DateTime? EndEffectiveDate = null) where EntityT : EntityBase
        {
            EntityT Instance = (EntityT)EntityBase.GetEntityBySystemType(typeof(EntityT));

            return (EntityT)CreateEntity(Instance.Type, EndEffectiveDate);
        }

        /// <summary>
        /// Creates an entity using the EntityCollection's EffectiveDate
        /// as start date and optionally an EndEffectiveDate if provided.
        /// </summary>
        /// <param name="EntityType">Type of the entity to create.</param>
        /// <param name="EndEffectiveDate">The end effective date.</param>
        /// <returns>The new Entity.</returns>
        public EntityBase CreateEntity(EntityType EntityType, DateTime? EndEffectiveDate = null)
        {
            if (EntityType == null)
            {
                throw new ArgumentNullException(nameof(EntityType));
            }
            var Entity = EntityBase.GetEntityByType(EntityType);
            Entity.EffectiveDate = EffectiveDate;
            Entity.EndEffectiveDate = EndEffectiveDate;

            Item.AddEntity(Entity);

            return Entity;
        }

        /// <summary>
        /// Creates the a new entity of the supplied type and immediately applies the entity's policy,
        /// using the passed strategies or the policy's default strategy if not provided.
        /// </summary>
        /// <typeparam name="EntityT">Type of the entity to create.</param>
        /// <param name="EndEffectiveDate">An optional end effective date for the entity.</param>
        /// <param name="CopyValuesFromPrevious">if set to <c>true</c>, try to find the closest entity of the same type going backwards in time and load the new entity with all its values.</param>
        /// <param name="PreferredStrategy">The preferred strategy.</param>
        /// <param name="PreferredStrategyForDuplicateDates">The preferred strategy for duplicate dates.</param>
        /// <returns>The new Entity</returns>
        public EntityT CreateEntityAndApplyPolicy<EntityT>(DateTime? EndEffectiveDate = null, bool CopyValuesFromPrevious = false, IUpdateStrategy PreferredStrategy = null, IUpdateStrategy PreferredStrategyForDuplicateDates = null)
            where EntityT : EntityBase
        {
            EntityT Instance = (EntityT)EntityBase.GetEntityBySystemType(typeof(EntityT));

            return (EntityT)CreateEntityAndApplyPolicy(Instance.Type, EndEffectiveDate, CopyValuesFromPrevious, PreferredStrategy, PreferredStrategyForDuplicateDates);
        }

        /// <summary>
        /// Creates the a new entity of the supplied type and immediately applies the entity's policy,
        /// using the passed strategies or the policy's default strategy if not provided.
        /// </summary>
        /// <param name="EntityType">Type of the entity to create.</param>
        /// <param name="EndEffectiveDate">An optional end effective date for the entity.</param>
        /// <param name="CopyValuesFromPrevious">if set to <c>true</c>, try to find the closest entity of the same type going backwards in time and load the new entity with all its values.</param>
        /// <param name="PreferredStrategy">The preferred strategy.</param>
        /// <param name="PreferredStrategyForDuplicateDates">The preferred strategy for duplicate dates.</param>
        /// <returns>The new Entity</returns>
        public EntityBase CreateEntityAndApplyPolicy(EntityType EntityType, DateTime? EndEffectiveDate = null, bool CopyValuesFromPrevious = false, IUpdateStrategy PreferredStrategy = null, IUpdateStrategy PreferredStrategyForDuplicateDates = null)
        {
            var ExistingEntities = Item.AllEntities.Where(e => e.Type == EntityType && e.EffectiveDate <= this.EffectiveDate).OrderBy(e => e.EffectiveDate);
            var MostRecent = ExistingEntities.LastOrDefault();
            var NewEntity = CreateEntity(EntityType, EndEffectiveDate);

            if (CopyValuesFromPrevious && MostRecent != null)
            {
                NewEntity.CopyValuesFrom(MostRecent);
            }

            NewEntity.Item.PerformUpdate(NewEntity, PreferredStrategy, PreferredStrategyForDuplicateDates);

            return NewEntity;
        }

        /// <summary>
        /// Gets or creates a new Entity.
        /// </summary>
        /// <typeparam name="EntityT">The type of the Entity.</typeparam>
        /// <param name="EndEffectiveDate">An optional end effective date of the new entity.</param>
        /// <returns></returns>
        public EntityT GetOrCreateEntity<EntityT>(DateTime? EndEffectiveDate = null) where EntityT : EntityBase
        {
            EntityT Instance = (EntityT)EntityBase.GetEntityBySystemType(typeof(EntityT), Item);

            return (EntityT)GetOrCreateEntity(Instance.Type, EndEffectiveDate);
        }

        public EntityBase GetOrCreateEntity(EntityType EntityType, DateTime? EndEffectiveDate = null)
        {
            var Entity = GetFirstEntityOrDefault(EntityType);

            if (Entity == null)
            {
                Entity = EntityBase.GetEntityByType(EntityType, Item);
                Entity.EffectiveDate = EffectiveDate;
                Entity.EndEffectiveDate = EndEffectiveDate;

                Item.AddEntity(Entity);
            }

            return Entity;
        }

        internal EntityT GetOrCreateEntityButDontSave<EntityT>(DateTime? EndEffectiveDate = null) where EntityT : EntityBase, new()
        {
            EntityT Instance = (EntityT)EntityBase.GetEntityBySystemType(typeof(EntityT), Item);

            return (EntityT)GetOrCreateEntityButDontSave(Instance.Type, EndEffectiveDate);
        }

        internal EntityBase GetOrCreateEntityButDontSave(EntityType EntityType, DateTime? EndEffectiveDate = null)
        {
            var Entity = GetFirstEntityOrDefault(EntityType);

            if (Entity == null)
            {
                Entity = EntityBase.GetEntityByType(EntityType);
                Entity.EffectiveDate = EffectiveDate;
                Entity.EndEffectiveDate = EndEffectiveDate;
            }

            return Entity;
        }
    }
}
