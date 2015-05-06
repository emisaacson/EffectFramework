using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Core.Services;
using Ninject;
using Ninject.Modules;

namespace HRMS.Core
{
    public class Configure : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IPersistenceService>()
                    .To<EntityFrameworkPersistenceService>();
        }
    }
}
