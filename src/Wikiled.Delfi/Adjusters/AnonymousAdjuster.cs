using System;
using System.Collections.Specialized;

namespace Wikiled.Delfi.Adjusters
{
    public class AnonymousAdjuster : IAdjuster
    {
        public void AddParametres(NameValueCollection parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            parameters["reg"] = "0";
        }
    }
}
