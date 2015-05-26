using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EffectFramework.Core.Models.Entities;

namespace EffectFramework.Core.Models
{
    public class AdjustPreferringNewStrategy : IUpdateStrategy
    {
        public void PerformUpdate(EntityBase CandidateEntity, EntityBase UpdatedEntity)
        {
            if (CandidateEntity == UpdatedEntity)
            {
                return;
            }

            if (CandidateEntity.Type != UpdatedEntity.Type)
            {
                return;
            }

            //if (CandidateEntity.EffectiveDate == UpdatedEntity.EffectiveDate)
        }
    }
}
