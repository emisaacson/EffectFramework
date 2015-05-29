using EffectFramework.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// A policy to ensure only one entity of a particular type is ever
    /// created for an item.
    /// </summary>
    public class SingletonPolicy : UpdatePolicy
    {
        private readonly IEnumerable<IUpdateStrategy> AvailableStrategies = new IUpdateStrategy[]
        {
            new ReplaceStrategy()
        };

        private readonly IUpdateStrategy DefaultStrategy = new ReplaceStrategy();
        private readonly IUpdateStrategy DefaultStrategyForDuplicateDates = new ReplaceStrategy();

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
