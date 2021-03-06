﻿using EffectFramework.Core.Models.Entities;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// A strategy to correct any overlap of entities of the same type, when there is a violation
    /// the newly created or updated entity always wins and the losing entity is deleted or has
    /// its effective dates altered to ensure sequential progression. If a losing entity exists
    /// on both sids of the effective period for the new entity, the losing entity is split into
    /// two to enclose the winning entity on both sides.
    /// </summary>
    public class AdjustPreferringNewStrategy : IUpdateStrategy
    {
        public void PerformUpdate(EntityBase CandidateEntity, EntityBase UpdatedEntity)
        {
            if (CandidateEntity == UpdatedEntity || CandidateEntity.Type != UpdatedEntity.Type || UpdatedEntity.IsDeleted)
            {
                return;
            }

            // To make this easier to understand we divide into two main
            // cases: end date of the new entity exists, or does not.
            if (!UpdatedEntity.EndEffectiveDate.HasValue)
            {
                // An then drill into the cases of the candidate entity.

                // Candidate starts after the updated entity
                if (CandidateEntity.EffectiveDate >= UpdatedEntity.EffectiveDate)
                {
                    CandidateEntity.Delete();
                }
                // Candidate starts before the updated entity but ends after
                // the updated starts
                else if (!CandidateEntity.EndEffectiveDate.HasValue ||
                    CandidateEntity.EndEffectiveDate.Value >= UpdatedEntity.EffectiveDate)
                {
                    CandidateEntity.EndEffectiveDate = UpdatedEntity.EffectiveDate;
                }
            }
            else
            {
                // Candidate is partially before and partially during the updated entity.
                // Adjust the end date.
                if (CandidateEntity.EffectiveDate < UpdatedEntity.EffectiveDate &&
                    CandidateEntity.EndEffectiveDate.HasValue &&
                    CandidateEntity.EndEffectiveDate.Value > UpdatedEntity.EffectiveDate &&
                    CandidateEntity.EndEffectiveDate.Value <= UpdatedEntity.EndEffectiveDate.Value)
                {
                    CandidateEntity.EndEffectiveDate = UpdatedEntity.EffectiveDate;
                }
                // Candidate is entirely during the updated entity. Delete
                else if (CandidateEntity.EffectiveDate >= UpdatedEntity.EffectiveDate && 
                    CandidateEntity.EndEffectiveDate.HasValue && CandidateEntity.EndEffectiveDate.Value <= UpdatedEntity.EndEffectiveDate.Value)
                {
                    CandidateEntity.Delete();
                }
                // Candidate is partially during and partially after the updated entity.
                // Adjust the start date
                else if (CandidateEntity.EffectiveDate >= UpdatedEntity.EffectiveDate &&
                    CandidateEntity.EffectiveDate < UpdatedEntity.EndEffectiveDate.Value &&
                    (!CandidateEntity.EndEffectiveDate.HasValue || CandidateEntity.EndEffectiveDate.Value > UpdatedEntity.EndEffectiveDate.Value))
                {
                    CandidateEntity.EffectiveDate = UpdatedEntity.EndEffectiveDate.Value;
                }
                // The candidate exists on both sides of the updated entity. Split into two.
                else if (CandidateEntity.EffectiveDate < UpdatedEntity.EffectiveDate &&
                    (!CandidateEntity.EndEffectiveDate.HasValue || CandidateEntity.EndEffectiveDate.Value > UpdatedEntity.EndEffectiveDate.Value))
                {
                    var OldCandidateEntityEndEffectiveDate = CandidateEntity.EndEffectiveDate;

                    CandidateEntity.EndEffectiveDate = UpdatedEntity.EffectiveDate;
                    EntityCollection EntityCollection = CandidateEntity.Item.GetEntityCollectionForDate(UpdatedEntity.EndEffectiveDate.Value);
                    var NewEntity = EntityCollection.CreateEntity(CandidateEntity.Type, OldCandidateEntityEndEffectiveDate);
                    NewEntity.CopyValuesFrom(CandidateEntity);

                }
            }
        }
    }
}
