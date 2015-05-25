using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models
{
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
