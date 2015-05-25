using EffectFramework.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models
{
    public interface IUpdateStrategy
    {
        void PerformUpdate(EntityBase CandidateEntity, EntityBase UpdatedEntity);
    }
}
