using EffectFramework.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// A base class that all Policies must inherit from.
    /// </summary>
    public abstract class UpdatePolicy
    {
        public abstract IEnumerable<IUpdateStrategy> GetAvailableStrategies();
        public abstract IUpdateStrategy GetDefaultStrategy();
        public abstract IUpdateStrategy GetDefaultStrategyForDuplicateDates();

        /// <summary>
        /// Given the provided strategies, manipulate all entities to correct any violations
        /// of the current policy. If no strategies are provided, the policy's default
        /// strategies are used. An exception is throws if the provided strategies are not
        /// valid for the current policy.
        /// </summary>
        /// <param name="Item">The item.</param>
        /// <param name="Entity">The entity.</param>
        /// <param name="PreferredUpdateStrategy">The preferred update strategy.</param>
        /// <param name="PreferredUpdateStrategyForDuplicateDates">The preferred update strategy for duplicate dates.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.InvalidOperationException">
        /// The requested strategy is not available for this policy.
        /// or
        /// The requested strategy is not available for this policy.
        /// </exception>
        public void PerformUpdate(Item Item, EntityBase Entity, IUpdateStrategy PreferredUpdateStrategy = null, IUpdateStrategy PreferredUpdateStrategyForDuplicateDates = null)
        {
            if (Item == null)
            {
                throw new ArgumentNullException(nameof(Item));
            }
            if (Entity == null)
            {
                throw new ArgumentNullException(nameof(Entity));
            }

            IUpdateStrategy Strategy = Entity.GetUpdateStrategy();
            IUpdateStrategy StrategyForDuplicateDates = Entity.GetUpdateStrategyForDuplicateDates();

            if (PreferredUpdateStrategy != null)
            {
                if (!Entity.GetUpdatePolicy().GetAvailableStrategies().Any(s => s.GetType() == PreferredUpdateStrategy.GetType()))
                {
                    throw new InvalidOperationException("The requested strategy is not available for this policy.");
                }

                Strategy = PreferredUpdateStrategy;
            }


            if (PreferredUpdateStrategyForDuplicateDates != null)
            {
                if (!Entity.GetUpdatePolicy().GetAvailableStrategies().Any(s => s.GetType() == PreferredUpdateStrategyForDuplicateDates.GetType()))
                {
                    throw new InvalidOperationException("The requested strategy is not available for this policy.");
                }

                StrategyForDuplicateDates = PreferredUpdateStrategyForDuplicateDates;
            }

            var AllEntities = Item.AllEntities.ToArray();
            for (int i = 0; i < AllEntities.Count(); i++)
            {
                var OtherEntity = AllEntities.ElementAt(i);

                if (OtherEntity.EffectiveDate == Entity.EffectiveDate && OtherEntity.EndEffectiveDate == Entity.EndEffectiveDate)
                {
                    StrategyForDuplicateDates.PerformUpdate(OtherEntity, Entity);
                }
                else
                {
                    Strategy.PerformUpdate(OtherEntity, Entity);
                }
            }
        }
    }
}
