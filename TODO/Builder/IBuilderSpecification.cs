using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Core.Builders
{
    public interface IBuilderSpecification
    {
        IEnumerable<object> GetAllItems(int EmpGeneralID);

    }
}
