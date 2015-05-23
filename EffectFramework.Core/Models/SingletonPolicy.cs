using EffectFramework.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models
{
    public class SingletonPolicy : UpdatePolicy
    {
        private readonly IEnumerable<IUpdateStrategy> AvailableStrategies = new IUpdateStrategy[]
        {
            new ReplaceStrategy()
        };

        private readonly IUpdateStrategy DefaultStrategy = new ReplaceStrategy();

        public override IEnumerable<IUpdateStrategy> GetAvailableStrategies()
        {
            return AvailableStrategies;
        }

        public override IUpdateStrategy GetDefaultStrategy()
        {
            return DefaultStrategy;
        }

    }
}
