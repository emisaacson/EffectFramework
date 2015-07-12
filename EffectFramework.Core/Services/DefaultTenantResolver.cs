namespace EffectFramework.Core.Services
{
    public class DefaultTenantResolver : ITenantResolutionProvider
    {
        public string GetTenantDatabase()
        {
            return "HRMS";
        }

        public long GetTenantID()
        {
            return 1;
        }
    }
}
