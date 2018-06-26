using System;
using System.Collections.Specialized;

namespace Wikiled.Delfi.Articles
{
    public class AnonymousAdjuster : IAdjuster
    {
        public void AddParametes(NameValueCollection parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            parameters["reg"] = "0";
        }
    }
}
