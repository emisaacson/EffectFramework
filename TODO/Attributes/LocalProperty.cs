using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Core.Attributes
{
    /*
     * This is a flag to show that the property on the Model
     * should be saved to the Local database. If the DB column
     * name does not match the property name, the column
     * name can be specified directly as a parameter on this
     * property.
     * 
     * Example:
     * 
     * [LocalProperty("EmpID")]
     * public int? EmployeeID { get; set; }
     * 
     * 
     */
    [AttributeUsage(AttributeTargets.Property)]
    public class LocalProperty : Attribute, IColumnSpecifier
    {
        public string ColumnName { get; private set; }
        public LocalProperty()
        {

        }

        public LocalProperty(string ColumnName)
        {
            this.ColumnName = ColumnName;
        }
    }
}