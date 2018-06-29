using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Delfi.Data;

namespace Wikiled.Delfi.Readers
{
    public class ArticleDataReader : IArticleDataReader
    {
        private readonly ILogger<ArticleDataReader> logger;

        private readonly ICommentsReaderFactory commentsReaderFactory;

        private readonly IArticleTextReader articleTextReader;

        public ArticleDataReader(ILoggerFactory loggerFactory, ICommentsReaderFactory commentsReaderFactory, IArticleTextReader articleTextReader)
        {
            logger = loggerFactory?.CreateLogger<ArticleDataReader>() ?? throw new ArgumentNullException(nameof(logger));
            this.commentsReaderFactory = commentsReaderFactory ?? throw new ArgumentNullException(nameof(commentsReaderFactory));
            this.articleTextReader = articleTextReader ?? throw new ArgumentNullException(nameof(articleTextReader));
        }

        public async Task<Article> Read(ArticleDefinition definition)
        {
            logger.LogDebug("Reading article: {0}[{1}]", definition.Title, definition.Id);
            var anonymous = ReadComments(definition, true);
            var registered = ReadComments(definition, false);
            var readArticle = articleTextReader.ReadArticle(definition);
            var anonymousResult = await anonymous;
            var registeredResult = await registered;
            return new Article(definition, registeredResult.Concat(anonymousResult).ToArray(), await readArticle);
        }

        private async Task<CommentData[]> ReadComments(ArticleDefinition definition, bool anonymous)
        {
            var commentsReader = commentsReaderFactory.Create(definition, anonymous);
            await commentsReader.Init();
            var result = await commentsReader.ReadAllComments().ToArray();
            return result;
        }
    }
}
