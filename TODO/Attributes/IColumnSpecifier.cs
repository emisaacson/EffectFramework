using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Core.Attributes
{
    /*
     * Interface for an attribute that specifies a column name.
     * 
     * This is a good way to be able to use MasterProperty or LocalProperty
     * without having to know which one it is necessarily, see references for
     * examples.
     * 
     */
    public interface IColumnSpecifier
    {
        string ColumnName { get; }
    }
}
