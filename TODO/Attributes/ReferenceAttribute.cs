using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Core.Attributes
{
    /*
     * This is an interface that allows you to use MasterReference
     * or LocalReference without know necessarily which one you're going
     * to get.
     */
    public abstract class ReferenceAttribute : Attribute
    {
        public abstract List<object> Items { get; }
        public string PropertyName { get; protected set; }
        public Type PropertyType { get; protected set; }
        public string KeyProperty { get; protected set; }
        public string ValueProperty { get; protected set; }
        public string[] QueryParams { get; protected set; } // :-(

        protected bool VerifyQueryParams(object Item)
        {
            for (int i = 0; i < QueryParams.Count() - 1; i += 2)
            {
                if (!(PropertyType.GetProperty(QueryParams[i]).GetValue(Item).ToString().Equals(QueryParams[i + 1]))) // :-(
                {
                    return false;
                }
            }

            return true;
        }
    }
}
