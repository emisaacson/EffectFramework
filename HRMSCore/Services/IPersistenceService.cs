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
        void SaveSingleField(EntityBase Entity, IField Field);

        FieldBase RetreiveSingleFieldOrDefault<FieldT>(EntityBase Entity) where FieldT : IField, new();

        EntityT RetreiveSingleEntityOrDefault<EntityT>(Models.EmployeeRecord EmployeeRecord) where EntityT : EntityBase, new();
        EntityT RetreiveSingleEntityOrDefault<EntityT>(int EmployeeRecordID) where EntityT : EntityBase, new();

    }
}
