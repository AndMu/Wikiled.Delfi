using System.Collections.Specialized;

namespace Wikiled.Delfi.Adjusters
{
    public interface IAdjuster
    {
        void AddParametes(NameValueCollection parameters);
    }
}