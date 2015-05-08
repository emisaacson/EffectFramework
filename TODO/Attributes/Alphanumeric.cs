using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HRMS.Core.Attributes
{
    public class Alphanumeric : RegularExpressionAttribute
    {
        public Alphanumeric()
            : base(@"^[a-zA-Z0-9]*$")
        {
        }
    }
}