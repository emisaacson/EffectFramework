using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Local.Classes.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited=false)]
    public class MasterTable : Attribute, ITableSpecifier
    {
        public string TablePluralName { get; set; }
        public Type TableType { get; set; }
        public MasterTable(string TablePluralName, Type TableType)
        {
            this.TablePluralName = TablePluralName;
            this.TableType = TableType;
        }
    }
}