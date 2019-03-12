using System;
using System.Collections.Specialized;

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
    }
}
