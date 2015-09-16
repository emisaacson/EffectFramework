using System;
using System.Collections.Generic;
using EffectFramework.Core.Models;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Models.Fields;

namespace EffectFramework.Core.Services
{
    /// <summary>
    /// An interface that any persistence service must use to provide object persistence for the framework.
    /// </summary>
    public interface IPersistenceService
    {
        /// <summary>
        /// Persist a single FieldBase object to the persistence service. The entity must be provided
        /// if the FieldID is not set.
        /// </summary>
        /// <param name="Entity">The entity where the field is located.</param>
        /// <param name="Field">The field.</param>
        /// <param name="ctx">An optional context (if a transaction has been initiated already, for instance.) If not provided,
        /// a new one will be created.</param>
        /// <returns>The FieldID and GUID of the saved field.</returns>
        Models.Db.ObjectIdentity SaveSingleField(EntityBase Entity, FieldBase Field, Models.Db.IDbContext ctx = null);

        /// <summary>
        /// Persist a single FieldBase object to the persistence service. The entity must be provided
        /// if the FieldID is not set.
        /// </summary>
        /// <param name="Field">The field.</param>
        /// <param name="ctx">An optional context (if a transaction has been initiated already, for instance.) If not provided,
        /// a new one will be created.</param>
        /// <returns>The FieldID and GUID of the saved field.</returns>
        Models.Db.ObjectIdentity SaveSingleField(FieldBase Field, Models.Db.IDbContext ctx = null);

        /// <summary>
        /// Retreives a Lookup Collection instance by ID
        /// </summary>
        /// <param name="LookupTypeID">The ID of the collection.</param>
        /// <param name="ctx">An optional context (if a transaction has been initiated already, for instance.) If not provided,
        /// a new one will be created.</param>
        /// <returns>A LookupCollection instance populated by the database.</returns>
        LookupCollection GetLookupCollectionById(long LookupTypeID, Models.Db.IDbContext ctx = null);

        IEnumerable<LookupCollection> GetAllLookupCollections(Models.Db.IDbContext ctx = null);

        Models.Db.ObjectIdentity SaveLookupCollection(LookupCollection LookupCollection, Models.Db.IDbContext ctx = null);

        IEnumerable<LookupEntry> GetLookupEntries(long LookupTypeID, LookupCollection LookupCollection, Models.Db.IDbContext ctx = null);

        Models.Db.ObjectIdentity SaveSingleLookupEntry(LookupEntry LookupEntry, Models.Db.IDbContext ctx = null);

        void SaveAndDeleteLookupEntry(LookupEntry LookupEntry, Models.Db.IDbContext ctx = null);

        void SaveAndDeleteLookupCollection(LookupCollection LookupCollection, Models.Db.IDbContext ctx = null);

        /// <summary>
        /// Persists a single entity to the database. If the EntityID is not available, the Item must be provided.
        /// </summary>
        /// <param name="Item">The item that the entity belongs to.</param>
        /// <param name="Entity">The entity.</param>
        /// <param name="ctx">An optional context (if a transaction has been initiated already, for instance.) If not provided,
        /// a new one will be created.</param>
        /// <returns>The EntityID and GUID of the saved entity.</returns>
        Models.Db.ObjectIdentity SaveSingleEntity(Item Item, EntityBase Entity, Models.Db.IDbContext ctx = null);

        /// <summary>
        /// Persists a single entity to the database. If the EntityID is not available, the Item must be provided.
        /// </summary>
        /// <param name="Entity">The entity.</param>
        /// <param name="ctx">An optional context (if a transaction has been initiated already, for instance.) If not provided,
        /// a new one will be created.</param>
        /// <returns>The EntityID and GUID of the saved entity.</returns>
        Models.Db.ObjectIdentity SaveSingleEntity(EntityBase Entity, Models.Db.IDbContext ctx = null);

        IFieldTypeMeta GetFieldTypeMeta(long ItemTypeID, long EntityTypeID, long FieldTypeID, Models.Db.IDbContext ctx = null);


        /// <summary>
        /// Persists a single item to the database.
        /// </summary>
        /// <param name="Item">The item.</param>
        /// <param name="ctx">An optional context (if a transaction has been initiated already, for instance.) If not provided,
        /// a new one will be created.</param>
        /// <returns>The ItemID and GUID of the saved Item.</returns>
        Models.Db.ObjectIdentity SaveSingleItem(Item Item, Models.Db.IDbContext ctx = null);

        /// <summary>
        /// Retreives a single field from the database of the specified type.
        /// </summary>
        /// <param name="Entity">An entity to which the field may belong.</param>
        /// <param name="FieldType">Type of the field.</param>
        /// <param name="ctx">An optional context (if a transaction has been initiated already, for instance.) If not provided,
        /// a new one will be created.</param>
        /// <returns>An instance of the field, or null if one cannot be found.</returns>
        FieldBase RetreiveSingleFieldOrDefault(EntityBase Entity, FieldType FieldType, Models.Db.IDbContext ctx = null);

        /// <summary>
        /// Retreives a single field from the database of the specified type ID.
        /// </summary>
        /// <param name="Entity">The entity.</param>
        /// <param name="FieldTypeID">The field type identifier.</param>
        /// <param name="ctx">An optional context (if a transaction has been initiated already, for instance.) If not provided,
        /// a new one will be created.</param>
        /// <returns>An instance of the field, or null if one cannot be found.</returns>
        FieldBase RetreiveSingleFieldOrDefault(EntityBase Entity, long FieldTypeID, Models.Db.IDbContext ctx = null);

        /// <summary>
        /// Retreives a single field from the database by field ID.
        /// </summary>
        /// <param name="FieldID">The field ID.</param>
        /// <param name="ctx">An optional context (if a transaction has been initiated already, for instance.) If not provided,
        /// a new one will be created.</param>
        /// <returns>An instance of the field, or null if it cannot be found.</returns>
        FieldBase RetreiveSingleFieldOrDefault(long FieldID, Models.Db.IDbContext ctx = null);

        /// <summary>
        /// Retreives a single field from the database of the specified type.
        /// </summary>
        /// <typeparam name="FieldT">The type of the field</typeparam>
        /// <param name="Entity">An entity to which the field may belong.</param>
        /// <param name="ctx">An optional context (if a transaction has been initiated already, for instance.) If not provided,
        /// a new one will be created.</param>
        /// <returns>An instance of the field, or null if one cannot be found.</returns>
        FieldBase RetreiveSingleFieldOrDefault<FieldT>(EntityBase Entity, Models.Db.IDbContext ctx = null) where FieldT : IField, new();

        /* Consider removing this
        /// <summary>
        /// Retreives the single entity or default.
        /// </summary>
        /// <typeparam name="EntityT">The type of the ntity t.</typeparam>
        /// <param name="Item">The item.</param>
        /// <param name="EffectiveDate">The effective date.</param>
        /// <returns></returns>
        EntityT RetreiveSingleEntityOrDefault<EntityT>(Item Item, DateTime? EffectiveDate = null) where EntityT : EntityBase, new(); */

        /// <summary>
        /// Retreives the particular entity from the database.
        /// </summary>
        /// <param name="EntityID">The entity ID</param>
        /// <param name="ctx">An optional context (if a transaction has been initiated already, for instance.) If not provided,
        /// a new one will be created.</param>
        /// <returns>The Entity object</returns>
        EntityBase RetreiveSingleEntityOrDefault(long EntityID, Models.Db.IDbContext ctx = null);

        /// <summary>
        /// Saves all fields of the entity and sets the delete flag.
        /// </summary>
        /// <param name="Entity">The entity.</param>
        /// <param name="ctx">An optional context (if a transaction has been initiated already, for instance.) If not provided,
        /// a new one will be created.</param>
        void SaveAndDeleteSingleEntity(EntityBase Entity, Models.Db.IDbContext ctx = null);

        /// <summary>
        /// Retreives the GUID for the item.
        /// </summary>
        /// <param name="Item">The item.</param>
        /// <param name="ctx">An optional context (if a transaction has been initiated already, for instance.) If not provided,
        /// a new one will be created.</param>
        /// <returns>The GUID for the item.</returns>
        Guid RetreiveGuidForItem(Item Item, Models.Db.IDbContext ctx = null);

        /// <summary>
        /// Retreives all entities for the given item.
        /// </summary>
        /// <param name="Item">The item.</param>
        /// <param name="EffectiveDate">An optional effective date. If none is provided, all
        /// entities are returned. Otherwise, only entities overlapping with the effective
        /// date are returned.</param>
        /// <param name="ctx">An optional context (if a transaction has been initiated already, for instance.) If not provided,
        /// a new one will be created.</param>
        /// <returns>A list of all non-deleted entities.</returns>
        List<EntityBase> RetreiveAllEntities(Item Item, DateTime? EffectiveDate = null, Models.Db.IDbContext ctx = null);

        /// <summary>
        /// Retreives complete items from the view for the specified ItemIDs
        /// </summary>
        /// <param name="ItemIDs">An enumerable of ItemIDs to fetch.</param>
        /// <param name="ctx">An optional context (if a transaction has been initiated already, for instance.) If not provided,
        /// a new one will be created.</param>
        /// <returns>All matching rows from the CompleteItems view</returns>
        IEnumerable<Models.Db.CompleteItem> RetreiveCompleteItems(IEnumerable<long> ItemIDs, Models.Db.IDbContext ctx = null);

        /// <summary>
        /// Records the field changes in the audit log
        /// </summary>
        /// <param name="Field">The field that is being updated</param>
        /// <param name="ItemID">An item ID to add as a reference</param>
        /// <param name="Comment">A comment to save with the audit</param>
        /// <param name="ctx">An optional context (if a transaction has been initiated already, for instance.) If not provided,
        /// a new one will be created.</param>
        void RecordAudit(FieldBase Field, long? ItemID, string Comment, Models.Db.IDbContext ctx = null);

        /// <summary>
        /// Records the entity changes in the audit log
        /// </summary>
        /// <param name="Entity">The Entity that is being updated</param>
        /// <param name="ItemID">An item ID to add as a reference</param>
        /// <param name="Comment">A comment to save with the audit</param>
        /// <param name="ctx">An optional context (if a transaction has been initiated already, for instance.) If not provided,
        /// a new one will be created.</param>
        void RecordAudit(EntityBase Entity, long? ItemID, string Comment, Models.Db.IDbContext ctx = null);

        /// <summary>
        /// Returns a parent Lookup by its ID
        /// </summary>
        /// <param name="ParentID">The Parent ID which must be retrieved</param>
        LookupEntry GetParentLookup(long? ParentID);

        /// <summary>
        /// Retreives an instance of the database context.
        /// </summary>
        /// <returns>An new database context.</returns>
        Models.Db.IDbContext GetDbContext();
    }
}
