using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMSCore.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldDescriptionAttribute : Attribute
    {
        public string Description { get; set; }
        public FieldDescriptionAttribute(string Description)
        {
            this.Description = Description;
        }
    }
}