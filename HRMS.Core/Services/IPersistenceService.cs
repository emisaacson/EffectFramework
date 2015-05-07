using System;
using System.Collections.Generic;
using HRMS.Core.Models;
using HRMS.Core.Models.Entities;
using HRMS.Core.Models.Fields;

namespace HRMS.Core.Services
{
    public interface IPersistenceService
    {
        Guid SaveSingleField(EntityBase Entity, FieldBase Field, Models.Db.IDbContext ctx = null);
        Guid SaveSingleField(FieldBase Field, Models.Db.IDbContext ctx = null);

        FieldBase RetreiveSingleFieldOrDefault(EntityBase Entity, FieldType FieldType);
        FieldBase RetreiveSingleFieldOrDefault(EntityBase Entity, int FieldTypeID);
        FieldBase RetreiveSingleFieldOrDefault(int FieldID);

        FieldBase RetreiveSingleFieldOrDefault<FieldT>(EntityBase Entity) where FieldT : IField, new();

        EntityT RetreiveSingleEntityOrDefault<EntityT>(Models.EmployeeRecord EmployeeRecord) where EntityT : EntityBase, new();
        EntityT RetreiveSingleEntityOrDefault<EntityT>(int EmployeeRecordID) where EntityT : EntityBase, new();

        List<EmployeeRecord> RetreiveAllEmployeeRecords(Employee Employee);
        List<EmployeeRecord> RetreiveAllEmployeeRecords(int EmployeeID);

        Models.Db.EmployeeRecord RetreiveSingleDbEmployeeRecord(int EmployeeRecordID);

        Guid RetreiveGuidForEmployeeRecord(int EmployeeID);

        List<EntityBase> RetreiveAllEntities(Models.EmployeeRecord EmployeeRecord);
        List<EntityBase> RetreiveAllEntities(int EmployeeRecordID);
    }
}
