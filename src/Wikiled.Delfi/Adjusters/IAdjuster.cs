using System.Collections.Specialized;
using Wikiled.News.Monitoring.Data;

namespace Wikiled.Delfi.Adjusters
{
    public interface IAdjuster
    {
        void AddParametres(NameValueCollection parameters);

        CommentData Transform(CommentData data);
    }
}