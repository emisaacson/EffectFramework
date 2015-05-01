using Local.Classes.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Local.Classes.Attributes
{
    /*
     * This is a key attribute that should be used on virtually all
     * properties on all models. It specifies what security object
     * is required to acces the 
     * 
     */
    [AttributeUsage(AttributeTargets.Property)]
    public class RequirePermission : Attribute, IMetadataAware
    {
        public static readonly string MetaKey = "RequiredPermission";
        public string Permission { get; private set; }
        public RequirePermission(string RequiredPermission)
        {
            this.Permission = RequiredPermission;
        }

        public void OnMetadataCreated(ModelMetadata Metadata)
        {
            Metadata.AdditionalValues[MetaKey] = Permission;
        }
    }
}