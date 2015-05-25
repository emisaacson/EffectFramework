using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models
{
    public class AllowOverlapPolicy : UpdatePolicy
    {
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
