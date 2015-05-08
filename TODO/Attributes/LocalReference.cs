using HRMS.Core.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Core.Attributes
{
    /*
     * This is an attribute to specify a property on a model is a simple foreign
     * key to another table. It's somewhat needlessly complex but gets the job done.
     * 
     * The first parameter, PropertyName, indicates the plural name of the entity
     * as it exists on the Local linq to sql class.
     * 
     * Example, to reference Contact Types, specify "ContactInfoTypes" on the first
     * parameter because that collection can be accessed via LocalDB.ContactInfoTypes.
     * 
     * The next parameter is the type of entity to expect in the collection, e.g.
     * typeof(ContactInfoType)
     * 
     * The next parameter is the Key property on the foreign table. Using the above
     * example, this would be "ContTypeID".
     * 
     * The next parameter is the Value property on the foreign table. Using the above
     * example, "ContTypeName".
     * 
     * The final parameter is unfortunate and due to the fact that attributes
     * are not first class objects in c# and we can only specify constants. This is
     * a string array, where the members are key / value pairs in the following order:
     * 
     * "Key", "Value", "Key", "Value, "Key"....
     * 
     * And each pair is added as a requirement on the Where clause of the query.
     * For example, to specify IsActive = 'Y' in the where clause, use
     * new string[] { "IsActive", "Y" }.
     * 
     * If anyone has a more elegant solution to this they will be the recipient
     * of substantial brownie points.
     * 
     * Example:
     * 
     * 
     * [LocalReference("EmpAttachmentTypes", typeof(EmpAttachmentType), "AttachTypeID", "TypeName", new string[] { "IsActive", "Y" })]
     * public int? AttachmentType { get; set; }
     * 
     */
    public class LocalReference : ReferenceAttribute, IMetadataAware
    {
        public static readonly string MetaKey = "LocalReferences";
        public override List<object> Items
        {
            get
            {
                var ContextKey = "LocalReference_" + PropertyName + "_" + KeyProperty + " _ " + ValueProperty;
                if (!HttpContext.Current.Items.Contains(ContextKey))
                {
                    using (var Local = DBHelper.GetLocalDB())
                    {
                        var PropertyInfo = HRMS.CoreGetType().GetProperty(PropertyName);
                        HttpContext.Current.Items[ContextKey] =
                            ((IEnumerable<object>)PropertyInfo.GetValue(Local))
                                                              .ToList()
                                                              .Where(i => VerifyQueryParams(i))
                                                              .ToList();
                    }
                }
                
                return (List<object>)HttpContext.Current.Items[ContextKey];
            }
        }

        public LocalReference(string PropertyName, Type PropertyType, string KeyProperty, string ValueProperty, string[] QueryParams = null /* :-( */)
        {
            this.PropertyName = PropertyName;
            this.PropertyType = PropertyType;
            this.KeyProperty = KeyProperty;
            this.ValueProperty = ValueProperty;
            this.QueryParams = QueryParams;  // :-(

        }

        public void OnMetadataCreated(ModelMetadata Metadata)
        {
            Metadata.AdditionalValues[MetaKey] = this;
        }
    }
}