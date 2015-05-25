using EffectFramework.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models
{
    public abstract class UpdatePolicy
    {
        public abstract IEnumerable<IUpdateStrategy> GetAvailableStrategies();
        public abstract IUpdateStrategy GetDefaultStrategy();
        public abstract IUpdateStrategy GetDefaultStrategyForDuplicateDates();

        public void PerformUpdate(Item Item, EntityBase Entity, IUpdateStrategy PreferredUpdateStrategy = null, IUpdateStrategy PreferredUpdateStrategyForDuplicateDates = null)
        {
            if (Item == null || Entity == null)
            {
                throw new ArgumentNullException();
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
