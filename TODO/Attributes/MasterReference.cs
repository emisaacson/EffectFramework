using HRMS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Core.Attributes
{
    /*
     * The exact equivalent of the LocalReference attribute, except applicable
     * to the master database. See the documentation for LocalReference for more information.
     */
    public class MasterReference : ReferenceAttribute, IMetadataAware
    {
        public static readonly string MetaKey = "MasterReferences";
        public override List<object> Items
        {
            get
            {
                var ContextKey = "MasterReference_" + PropertyName + "_" + KeyProperty + " _ " + ValueProperty;
                if (!HttpContext.Current.Items.Contains(ContextKey))
                {
                    using (var Master = DBHelper.GetMasterDB())
                    {
                        var PropertyInfo = Master.GetType().GetProperty(PropertyName);
                        HttpContext.Current.Items[ContextKey] =
                            ((IEnumerable<object>)PropertyInfo.GetValue(Master))
                                                              .ToList()
                                                              .Where(i => VerifyQueryParams(i))
                                                              .ToList();
                    }
                }

                return (List<object>)HttpContext.Current.Items[ContextKey];
            }
        }

        public MasterReference(string PropertyName, Type PropertyType, string KeyProperty, string ValueProperty, string[] QueryParams = null /* :-( */)
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