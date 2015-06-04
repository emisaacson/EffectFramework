using System;
using System.Collections.Generic;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// This policy allows overlapping entities only if they share the exact effective and
    /// end effective dates. Otherwise, make sure the changes are sequential.
    /// </summary>
    public class AllowOverlapIfDuplicateDatesPolicy : UpdatePolicy
    {
        private readonly IEnumerable<IUpdateStrategy> AvailableStrategies = new IUpdateStrategy[]
        {
            new ReplaceStrategy(),
            new OverwriteStrategy(),
            new AdjustPreferringExistingStrategy(),
            new AdjustPreferringNewStrategy(),
            new AllowOverlapStrategy(),
        };
        private readonly IUpdateStrategy DefaultStrategy = new AdjustPreferringNewStrategy();
        private readonly IUpdateStrategy DefaultStrategyForDuplicateDates = new AllowOverlapStrategy();
        public override IEnumerable<IUpdateStrategy> GetAvailableStrategies()
        {
            throw new NotImplementedException();
        }

        public override IUpdateStrategy GetDefaultStrategy()
        {
            throw new NotImplementedException();
        }

        public override IUpdateStrategy GetDefaultStrategyForDuplicateDates()
        {
            throw new NotImplementedException();
        }
    }
}
