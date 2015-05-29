using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EffectFramework.Core.Models.Entities;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// This strategy will delete the all other entities of the same type
    /// effectively replacing them with the new or updated entity.
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
