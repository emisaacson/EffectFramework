using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Core.Models.Entities;
using HRMS.Core.Models.Fields;

namespace HRMS.Core.Services
{
    public interface IPersistenceService
    {
        void SaveSingleField(IEntity Entity, IField Field);

        FieldT RetreiveSingleFieldOrDefault<FieldT>(IEntity Entity) where FieldT : IField, new();
    }
}
