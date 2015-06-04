using System;
using EffectFramework.Core.Models.Entities;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// A strategy to correct overlapping entities by allowing any existing entity to
    /// remain unaltered, but chopping up the new entity to fill in any gaps that exists
    /// between the start and end effective dates of the new entity. Not very useful in
    /// most cases and not yet implemented.
    /// </summary>
    public class AdjustPreferringExistingStrategy : IUpdateStrategy
    {
        public void PerformUpdate(EntityBase CandidateEntity, EntityBase UpdatedEntity)
        {
            throw new NotImplementedException();
        }
    }
}
