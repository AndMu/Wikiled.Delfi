using System;
using System.Collections.Specialized;
using Wikiled.News.Monitoring.Data;

namespace Wikiled.Delfi.Adjusters
{
    public class RegisteredAdjuster : IAdjuster
    {
        public void AddParametres(NameValueCollection parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            parameters["reg"] = "1";
        }

        public CommentData Transform(CommentData data)
        {
            data.IsSpecialAuthor = true;
            return data;
        }
    }
}
