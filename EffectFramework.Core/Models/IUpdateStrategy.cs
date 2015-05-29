using EffectFramework.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// The interface that all strategies must implement.
    /// </summary>
    public interface IUpdateStrategy
    {
        /// <summary>
        /// Given two entities, test if there is a violation of the policy
        /// and if so correct it in an implementation-dependent manner.
        /// </summary>
        /// <param name="CandidateEntity">The existing entity that will be inspected for policy compliance.</param>
        /// <param name="UpdatedEntity">The new or updated entity triggering the compliance check.</param>
        void PerformUpdate(EntityBase CandidateEntity, EntityBase UpdatedEntity);
    }
}
