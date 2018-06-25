using System.Collections.Specialized;
using Microsoft.Extensions.Logging;

namespace Wikiled.Delfi.Articles
{
    public class RegisteredCommentsReader : CommentsReader
    {
        public RegisteredCommentsReader(ArticleDefinition article, ILogger<RegisteredCommentsReader> logger) 
            : base(article, logger)
        {
        }

        protected override void AddParametes(NameValueCollection parameters)
        {
            parameters["reg"] = "1";
        }
    }
}
