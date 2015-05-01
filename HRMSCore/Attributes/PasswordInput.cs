using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Local.Classes.Attributes
{
    /*
     * Specifies that the Model property should be rendered with
     * an <input type="password" /> element.
     * 
     */
    [AttributeUsage(AttributeTargets.Property)]
    public class PasswordInput : Attribute
    {

    }
}