namespace EffectFramework.Core.Services
{
    public class DefaultTenantResolver : ITenantResolutionProvider
    {
        public string GetTenantDatabase()
        {
            return "EffectFramework";
        }

        public long GetTenantID()
        {
            return 1;
        }
    }
}
