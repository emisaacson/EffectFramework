using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EffectFramework.Core.Models.Entities;

namespace EffectFramework.Core.Models
{
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
