

namespace EffectFramework.Core.Models
{
    public interface IObjectQueryProvider
    {
        Item Item { get; set; }
        string QueryText { get; set; }
        bool ItemMatches();
    }
}
