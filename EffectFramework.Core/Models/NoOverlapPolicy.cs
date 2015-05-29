using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// This policy allows any number of of entities of a particular type
    /// for a particular item, but does not allow any overlap of effective
    /// dates. This is useful for modeling sequential time series data.
    /// </summary>
    public class NoOverlapPolicy : UpdatePolicy
    {
        private readonly IEnumerable<IUpdateStrategy> AvailableStrategies = new IUpdateStrategy[]
        {
            new ReplaceStrategy(),
            new OverwriteStrategy(),
            new AdjustPreferringExistingStrategy(),
            new AdjustPreferringNewStrategy(),
        };
        public override IEnumerable<IUpdateStrategy> GetAvailableStrategies()
        {
            return AvailableStrategies;
        }

        public override IUpdateStrategy GetDefaultStrategy()
        {
            return new AdjustPreferringNewStrategy();
        }

        public override IUpdateStrategy GetDefaultStrategyForDuplicateDates()
        {
            return new ReplaceStrategy();
        }
    }
}
