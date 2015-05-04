using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Core.Helpers
{
    public class ModelHelper
    {
        public static T GetDefaultValueOrNull<T>(Object Model, ViewDataDictionary<T> ViewData)
        {
            var Default = ViewData.ModelMetadata
                            .ContainerType
                            .GetProperty(ViewData.ModelMetadata.PropertyName)
                            .GetCustomAttributes(false)
                            .OfType<DefaultValueAttribute>()
                            .FirstOrDefault();

            if (Default != null)
            {
                try {
                    return (T)Default.Value;
                }
                catch (Exception)
                {
                    // e.g.: can't cast an Int to a Decimal. TODO: is the previous one
                    // necessary or does the below cover all cases?

                    var TargetType = typeof(T);

                    if (TargetType.IsGenericType && TargetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {

                        TargetType = Nullable.GetUnderlyingType(TargetType); ;
                    }

                    return (T)Convert.ChangeType(Default.Value, TargetType);
                }
            }
            else
            {
                return default(T);
            }
        }
    }
}