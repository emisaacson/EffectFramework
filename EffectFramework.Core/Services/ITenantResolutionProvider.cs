namespace EffectFramework.Core.Services
{
    public interface ITenantResolutionProvider
    {
        int GetTenantID();
        string GetTenantDatabase();
    }
}
