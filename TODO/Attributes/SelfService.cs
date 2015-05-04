using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace HRMS.Core.Attributes
{
    public class SelfService : Attribute
    {

        public bool IsReadOnly { get; private set; }

        public SelfService(bool ReadOnly = false)
        {
            this.IsReadOnly = ReadOnly;
        }
    }
}