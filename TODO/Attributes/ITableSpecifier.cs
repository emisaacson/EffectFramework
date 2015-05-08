using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Core.Attributes
{
    /*
     * Attributes that specify a table name implement this interface.
     * E.g.: LocalTable and MasterTable
     */
    public interface ITableSpecifier
    {
        string TablePluralName { get; set; }
        Type TableType { get; set; }
    }
}
