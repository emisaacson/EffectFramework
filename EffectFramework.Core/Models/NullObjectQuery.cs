namespace EffectFramework.Core.Models
{
    /// <summary>
    /// Default implementation of IObjectQueryProvider. Always
    /// returns false.
    /// </summary>
    public class NullObjectQuery : IObjectQueryProvider
    {
        public Item Item
        {
            get
            {
                return null;
            }
            set
            {

            }
        }

        public string QueryText
        {
            get
            {
                return null;
            }

            set
            {
                
            }
        }

        public bool ItemMatches()
        {
            return false;
        }
    }
}
