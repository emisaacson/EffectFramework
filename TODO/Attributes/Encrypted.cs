using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    public class Encrypted : Attribute
    {
        public static readonly string MetaKey = "Encrypted";
        private bool isVisible;
        public bool IsVisible
        {
            get {
                return isVisible;
            }
            set
            {
                isVisible = value;
            }
        }
    }
}