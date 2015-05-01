using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Local.Classes.Builders
{
    public interface IBuilderSpecification
    {
        IEnumerable<object> GetAllItems(int EmpGeneralID);

    }
}
