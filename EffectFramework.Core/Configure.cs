using EffectFramework.Core.Services;
using Ninject.Modules;

namespace EffectFramework.Core
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
