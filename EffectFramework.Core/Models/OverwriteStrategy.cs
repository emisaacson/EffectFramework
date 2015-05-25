using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EffectFramework.Core.Models.Entities;

namespace EffectFramework.Core.Models
{
    public class OverwriteStrategy : IUpdateStrategy
    {
        public void PerformUpdate(EntityBase CandidateEntity, EntityBase UpdatedEntity)
        {
            throw new NotImplementedException();
        }
    }
}
