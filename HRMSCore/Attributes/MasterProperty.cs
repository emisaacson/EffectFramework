using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Local.Classes.Attributes
{
    /*
     * This is a flag to show that the property on the Model
     * should be saved to the Master database. If the DB column
     * name does not match the property name, the column
     * name can be specified directly as a parameter on this
     * property.
     * 
     * Example:
     * 
     * [MasterProperty("LNAlias")]
     * public string LastNameAlias { get; set; }
     * 
     * 
     */
    [AttributeUsage(AttributeTargets.Property)]
    public class MasterProperty : Attribute, IColumnSpecifier
    {
        public string ColumnName { get; private set; }
        public MasterProperty()
        {

        }
        public MasterProperty(string ColumnName)
        {
            this.ColumnName = ColumnName;
        }
    }
}