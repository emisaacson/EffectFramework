using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EffectFramework.Core.Models.Entities;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// This strategy will delete the other entity to replace it
    /// with the new one
    /// </summary>
    public class OverwriteStrategy : IUpdateStrategy
    {
        public void PerformUpdate(EntityBase CandidateEntity, EntityBase UpdatedEntity)
        {
            if (CandidateEntity == UpdatedEntity || CandidateEntity.Type != UpdatedEntity.Type || UpdatedEntity.IsDeleted)
            {
                return;
            }

            CandidateEntity.Delete();

        }
    }
}
