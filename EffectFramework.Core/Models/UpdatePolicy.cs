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

        public void PerformUpdate(EntityCollection EntityCollection, EntityBase Entity, IUpdateStrategy PreferredUpdateStrategy = null)
        {
            if (EntityCollection == null || Entity == null)
            {
                throw new ArgumentNullException();
            }

            IUpdateStrategy Strategy = GetDefaultStrategy();

            if (PreferredUpdateStrategy != null)
            {
                if (!GetAvailableStrategies().Any(s => s.GetType() == PreferredUpdateStrategy.GetType()))
                {
                    throw new InvalidOperationException("The requested strategy is not available for this policy.");
                }

                Strategy = PreferredUpdateStrategy;
            }

            Strategy.PerformUpdate(EntityCollection, Entity);
        }
    }
}
