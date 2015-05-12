using System;
using System.Collections.Generic;
using System.Linq;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Services;

namespace EffectFramework.Core.Models
{
    public class EntityCollection
    {

        public IEnumerable<EntityBase> AllEntities
        {
            get
            {
                return Item.AllEntities
                    .Where(e =>
                        e.EffectiveDate >= this.EffectiveDate &&
                        (!e.EndEffectiveDate.HasValue || e.EndEffectiveDate < this.EffectiveDate)).AsEnumerable();
            }
        }

        private readonly IPersistenceService PersistenceService;

        public DateTime EffectiveDate { get; private set; }

        public Item Item { get; private set; }

        internal EntityCollection(Item Item, DateTime EffectiveDate, IPersistenceService PersistenceService)
        {
            this.Item = Item;
            this.EffectiveDate = EffectiveDate;
            this.PersistenceService = PersistenceService;
        }

        public EntityT GetFirstEntityOrDefault<EntityT>() where EntityT : EntityBase, new()
        {
            EntityT Instance = new EntityT();

            var Entity = AllEntities.Where(e => e.Type == Instance.Type).OrderBy(e => e.EffectiveDate).FirstOrDefault();

            if (Entity == null)
            {
                return default(EntityT);
            }

            return (EntityT)Entity;
        }

        public IEnumerable<EntityT> GetAllEntitiesOfType<EntityT>() where EntityT : EntityBase, new()
        {
            EntityT Instance = new EntityT();

            var Entities = AllEntities.Where(e => e.Type == Instance.Type).OrderBy(e => e.EffectiveDate).AsEnumerable();

            return (IEnumerable<EntityT>)Entities;
        }

        public EntityBase GetFirstEntityOrDefault(EntityType EntityType)
        {
            return AllEntities.Where(e => e.Type == EntityType).OrderBy(e => e.EffectiveDate).FirstOrDefault();
        }

        public IEnumerable<EntityBase> GetAllEntitiesOfType(EntityType EntityType)
        {
            return AllEntities.Where(e => e.Type == EntityType).OrderBy(e => e.EffectiveDate).AsEnumerable();
        }

        public EntityT GetOrCreateEntity<EntityT>(DateTime? EndEffectiveDate = null) where EntityT : EntityBase, new()
        {
            EntityT Instance = new EntityT();

            var Entity = GetFirstEntityOrDefault<EntityT>();

            if (Entity == null)
            {
                Entity = Instance;
                Entity.EffectiveDate = EffectiveDate;
                Entity.EndEffectiveDate = EndEffectiveDate;
                Entity.PersistenceService = PersistenceService;

                Item.AddEntity(Entity);
            }

            return Entity;
        }

        public EntityBase GetOrCreateEntity(EntityType EntityType, DateTime? EndEffectiveDate = null)
        {
            var Entity = GetFirstEntityOrDefault(EntityType);

            if (Entity == null)
            {
                Entity = (EntityBase)Activator.CreateInstance(EntityType.Type);
                Entity.EffectiveDate = EffectiveDate;
                Entity.EndEffectiveDate = EndEffectiveDate;
                Entity.PersistenceService = PersistenceService;

                Item.AddEntity(Entity);
            }

            return Entity;
        }

        internal EntityT GetOrCreateEntityButDontSave<EntityT>(DateTime? EndEffectiveDate = null) where EntityT : EntityBase, new()
        {
            EntityT Instance = new EntityT();

            var Entity = GetFirstEntityOrDefault<EntityT>();

            if (Entity == null)
            {
                Entity = Instance;
                Entity.EffectiveDate = EffectiveDate;
                Entity.EndEffectiveDate = EndEffectiveDate;
                Entity.PersistenceService = PersistenceService;
            }

            return Entity;
        }

        internal EntityBase GetOrCreateEntityButDontSave(EntityType EntityType, DateTime? EndEffectiveDate = null)
        {
            var Entity = GetFirstEntityOrDefault(EntityType);

            if (Entity == null)
            {
                Entity = (EntityBase)Activator.CreateInstance(EntityType.Type);
                Entity.EffectiveDate = EffectiveDate;
                Entity.EndEffectiveDate = EndEffectiveDate;
                Entity.PersistenceService = PersistenceService;
            }

            return Entity;
        }
    }
}
