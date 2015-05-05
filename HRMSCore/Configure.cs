using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Core.Services;
using Ninject;

namespace HRMS.Core
{
    public static class Configure
    {
        public static void WireEntityFramework7()
        {
            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<IPersistenceService>()
                      .To<EntityFrameworkPersistenceService>()
                      .InSingletonScope();
            }
        }
    }
}
