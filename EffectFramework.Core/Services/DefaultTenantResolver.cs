namespace EffectFramework.Core.Services
{
    public class DefaultTenantResolver : ITenantResolutionProvider
    {
        public string GetTenantDatabase()
        {
            return "HRMS";
        }

        public int GetTenantID()
        {
            return 1;
        }
    }
}
