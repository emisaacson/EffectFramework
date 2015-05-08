using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Core.Attributes
{
    /*
     * A flag that the attribute should be rendered as a
     * select2 multi-select field. Accepts the choices
     * of items in the same way that its base clas does.
     * 
     */
    [AttributeUsage(AttributeTargets.Property)]
    public class MultiChoice : Choices
    {
        public MultiChoice()
            : base()
        {
        }

        public MultiChoice(params string[] Items)
            : base(Items)
        {
        }

        public MultiChoice(Type ItemsType, string Field)
            : base(ItemsType, Field)
        {
        }
    }
}