using HRMS.Core.Services;
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
