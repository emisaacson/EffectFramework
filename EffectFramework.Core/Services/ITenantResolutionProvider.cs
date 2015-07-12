namespace EffectFramework.Core.Services
{
    public interface ITenantResolutionProvider
    {
        long GetTenantID();
        string GetTenantDatabase();
    }
}
