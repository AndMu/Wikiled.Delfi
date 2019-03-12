using System.Collections.Specialized;

namespace Wikiled.Delfi.Adjusters
{
    public interface IAdjuster
    {
        void AddParametres(NameValueCollection parameters);
    }
}