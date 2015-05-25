﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EffectFramework.Core.Models.Entities;

namespace EffectFramework.Core.Models
{
    /// <summary>
    /// The strategy will find all entities of the same type in the collection and update them
    /// with all values from the new or updated entity, deleting the new or updated entity.
    /// </summary>
    public class ReplaceStrategy : IUpdateStrategy
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

            CandidateEntity.CopyValuesFrom(UpdatedEntity);
            UpdatedEntity.Delete();

        }
    }
}
