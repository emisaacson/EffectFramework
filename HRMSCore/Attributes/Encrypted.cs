using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMSCore.Attributes
{
    /*
     * A flag on Model properties showing that the field is encrypted. This filters
     * the getters and setters through the encryption/decryption process automatically.
     * 
     * See ModelBuilderBase to see how this property is taken into account.
     * 
     */ 
    [AttributeUsage(AttributeTargets.Property)]
    public class Encrypted : Attribute, IMetadataAware
    {
        public static readonly string MetaKey = "Encrypted";
        private bool isVisible;
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
            }
        }

        public void OnMetadataCreated(ModelMetadata Metadata)
        {
            Metadata.AdditionalValues[MetaKey] = isVisible;
        }
    }
}