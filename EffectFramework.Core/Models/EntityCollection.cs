﻿using System;
using System.Collections.Generic;
using System.Linq;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Models.Fields;
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
                        e.EffectiveDate <= this.EffectiveDate &&
                        (!e.EndEffectiveDate.HasValue || e.EndEffectiveDate > this.EffectiveDate)).AsEnumerable();
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

            var Entities = AllEntities.Where(e => e.Type == Instance.Type).OrderBy(e => e.EffectiveDate).Cast<EntityT>();

            return Entities;
        }

        public EntityBase GetFirstEntityOrDefault(EntityType EntityType)
        {
            return AllEntities.Where(e => e.Type == EntityType).OrderBy(e => e.EffectiveDate).FirstOrDefault();
        }

        public IEnumerable<EntityBase> GetAllEntitiesOfType(EntityType EntityType)
        {
            return AllEntities.Where(e => e.Type == EntityType).OrderBy(e => e.EffectiveDate).AsEnumerable();
        }

        // EITODO: Test if we really can create one or not.
        public EntityT CreateEntity<EntityT>(DateTime? EndEffectiveDate = null) where EntityT : EntityBase, new()
        {
            EntityT Entity = new EntityT();

            Entity.EffectiveDate = EffectiveDate;
            Entity.EndEffectiveDate = EndEffectiveDate;
            Entity.PersistenceService = PersistenceService;

            Item.AddEntity(Entity);

            return Entity;
        }

        public EntityBase CreateEntity(EntityType EntityType, DateTime? EndEffectiveDate = null)
        {

            var Entity = (EntityBase)Activator.CreateInstance(EntityType.Type);
            Entity.EffectiveDate = EffectiveDate;
            Entity.EndEffectiveDate = EndEffectiveDate;
            Entity.PersistenceService = PersistenceService;

            Item.AddEntity(Entity);

            return Entity;
        }

        public EntityT CreateEntityAndEndDateAllPrevious<EntityT>(bool CopyValuesFromPrevious = false, DateTime? EndEffectiveDate = null) where EntityT : EntityBase, new()
        {
            var ExistingEntities = GetAllEntitiesOfType<EntityT>();
            var MostRecent = ExistingEntities.LastOrDefault();

            foreach (var ExistingEntity in ExistingEntities)
            {
                ExistingEntity.EndEffectiveDate = EffectiveDate;
            }

            EntityT Entity = new EntityT();

            Entity.EffectiveDate = EffectiveDate;
            Entity.EndEffectiveDate = EndEffectiveDate;
            Entity.PersistenceService = PersistenceService;

            if (CopyValuesFromPrevious && MostRecent != null)
            {
                var Properties = typeof(EntityT).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                foreach (var Property in Properties)
                {
                    if (typeof(IField).IsAssignableFrom(Property.PropertyType))
                    {
                        IField PreviousField = (IField)Property.GetValue(MostRecent);
                        IField NewField = (IField)Property.GetValue(Entity);
                        NewField.Value = PreviousField.Value;
                    }
                }
            }

            Item.AddEntity(Entity);

            return Entity;
        }

        public EntityBase CreateEntityAndEndDateAllPrevious(EntityType EntityType, bool CopyValuesFromPrevious = false, DateTime? EndEffectiveDate = null)
        {
            var ExistingEntities = GetAllEntitiesOfType(EntityType);
            var MostRecent = ExistingEntities.LastOrDefault();

            foreach (var ExistingEntity in ExistingEntities)
            {
                ExistingEntity.EndEffectiveDate = EffectiveDate;
            }

            var Entity = (EntityBase)Activator.CreateInstance(EntityType.Type);
            Entity.EffectiveDate = EffectiveDate;
            Entity.EndEffectiveDate = EndEffectiveDate;
            Entity.PersistenceService = PersistenceService;

            if (CopyValuesFromPrevious && MostRecent != null)
            {
                var Properties = EntityType.Type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                foreach (var Property in Properties)
                {
                    if (typeof(IField).IsAssignableFrom(Property.PropertyType))
                    {
                        IField PreviousField = (IField)Property.GetValue(MostRecent);
                        IField NewField = (IField)Property.GetValue(Entity);
                        NewField.Value = PreviousField.Value;
                    }
                }
            }

            Item.AddEntity(Entity);

            return Entity;
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
