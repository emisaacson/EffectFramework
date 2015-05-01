using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Local.Classes.Attributes
{
    public class SelfService : Attribute, IMetadataAware
    {
        public static readonly string MetaKey = "SelfService";

        public bool IsReadOnly { get; set; }

        public SelfService(bool ReadOnly = false)
        {
            this.IsReadOnly = ReadOnly;
        }

        public void OnMetadataCreated(ModelMetadata Metadata)
        {
            Metadata.AdditionalValues[MetaKey] = IsReadOnly;
        }
    }
}