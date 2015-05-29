using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// A permissive policy to allow any overlap of entities.
    /// </summary>
    public class AllowOverlapPolicy : UpdatePolicy
    {
        private readonly IEnumerable<IUpdateStrategy> AvailableStrategies = new IUpdateStrategy[]
        {
            new AllowOverlapStrategy(),
        };
        private readonly IUpdateStrategy DefaultStrategy = new AllowOverlapStrategy();
        private readonly IUpdateStrategy DefaultStrategyForDuplicateDates = new AllowOverlapStrategy();

        public override IEnumerable<IUpdateStrategy> GetAvailableStrategies()
        {
            return AvailableStrategies;
        }

        public override IUpdateStrategy GetDefaultStrategy()
        {
            return DefaultStrategy;
        }

        public override IUpdateStrategy GetDefaultStrategyForDuplicateDates()
        {
            return DefaultStrategyForDuplicateDates;
        }
    }
}
