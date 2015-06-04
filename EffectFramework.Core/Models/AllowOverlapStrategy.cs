using EffectFramework.Core.Models.Entities;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// Permissive strategy to allow any overlapping entities
    /// </summary>
    public class AllowOverlapStrategy : IUpdateStrategy
    {
        /// <summary>
        /// A very simple implementation ;-)
        /// </summary>
        /// <param name="CandidateEntity">The candidate entity.</param>
        /// <param name="UpdatedEntity">The updated entity.</param>
        public void PerformUpdate(EntityBase CandidateEntity, EntityBase UpdatedEntity)
        {
            // Live and let live.
        }
    }
}
