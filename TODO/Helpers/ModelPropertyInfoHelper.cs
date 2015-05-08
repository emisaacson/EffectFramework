using HRMS.Core.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Core.Helpers
{
    public class ModelPropertyInfoHelper
    {
        public static string GetPropertyDisplayName(PropertyInfo property)
        {
            if (property == null)
                return String.Empty;
            var displayNameAttr = property.GetCustomAttributes(false)
                                      .OfType<DisplayNameAttribute>()
                                      .FirstOrDefault();
            string FieldName;

            if (displayNameAttr != null)
            {
                FieldName = ((DisplayNameAttribute)displayNameAttr).DisplayName;
            }
            else
            {
                FieldName = property.Name;
            }
            return FieldName;
        }

        public static string GetModelDisplayName(PropertyInfo property)
        {
            if (property == null)
                return String.Empty;
            var DisplayNameAttribute = property.DeclaringType.GetCustomAttributes()
                                          .OfType<DisplayNameAttribute>()
                                          .FirstOrDefault();
            string SectionName = property.DeclaringType.Name;
            if (DisplayNameAttribute != null)
            {
                SectionName = DisplayNameAttribute.DisplayName;
            }
            return SectionName;

        }

        public static string GetPropertyDescription(PropertyInfo property,ModelMetadata Metadata)
        {
            if (property == null)
                return String.Empty;
            FieldDescriptionAttribute descrAttr = Metadata.ContainerType
                                                  .GetProperty(Metadata.PropertyName)
                                                  .GetCustomAttributes(false)
                                                  .OfType<FieldDescriptionAttribute>()
                                                  .FirstOrDefault();
            string Description = string.Empty;
            if (descrAttr != null)
            {
                Description = descrAttr.Description;
            }
            return Description;
        }

        public static FieldInfo GetSSFieldInfo(PropertyInfo property)
        {
            if (property == null)
                return new FieldInfo() { IsAvailableInSS = false, IsReadOnlyInSS = true };
            string SectionName = GetModelDisplayName(property);
            string FieldName = GetPropertyDisplayName(property);

            HRMS.Core.Models.ModelsContext.FACL[] facl = DBHelper.GetCurrentUserContext().GetUserFACLArray();
            HRMS.Core.Models.ModelsContext.FACL field = facl.FirstOrDefault(x => x.FieldName == FieldName && x.Modelname == SectionName);
            FieldInfo result = new FieldInfo() { IsAvailableInSS = false, IsReadOnlyInSS = true };
            if(field!=null)
            {
                result.IsAvailableInSS = field.IsSelfService;
                result.IsReadOnlyInSS = field.IsReadOnlyInSS;
                result.IsReadOnly = field.IsReadOnly;
            }
            return result;
        }
    }

    public class FieldInfo
    {
        public bool IsAvailableInSS { get; set; }
        public bool IsReadOnlyInSS { get; set; }
        public bool IsReadOnly { get; set; }
    }
}