using System.Collections.Specialized;

namespace Wikiled.Delfi.Articles
{
    public interface IAdjuster
    {
        void AddParametes(NameValueCollection parameters);
    }
}