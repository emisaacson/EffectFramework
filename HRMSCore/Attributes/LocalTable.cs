using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Local.Classes.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited=false)]
    public class LocalTable : Attribute, ITableSpecifier
    {
        public string TablePluralName { get; set; }
        public Type TableType { get; set; }
        public LocalTable(string TablePluralName, Type TableType)
        {
            this.TablePluralName = TablePluralName;
            this.TableType = TableType;
        }
    }
}