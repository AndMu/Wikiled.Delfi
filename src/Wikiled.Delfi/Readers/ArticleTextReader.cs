using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.Delfi.Readers
{
    public class ArticleTextReader : IArticleTextReader
    {
        private readonly ILogger<ArticleTextReader> logger;

        private readonly ITrackedRetrieval reader;

        public ArticleTextReader(ILoggerFactory loggerFactory, ITrackedRetrieval reader)
        {
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            logger = loggerFactory?.CreateLogger<ArticleTextReader>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ArticleContent> ReadArticle(ArticleDefinition definition, CancellationToken token)
        {
            logger.LogDebug("Reading article text: {0}", definition.Id);
            var page = (await reader.Read(definition.Url, token).ConfigureAwait(false)).GetDocument();
            var doc = page.DocumentNode;
            var article = doc.QuerySelector("div.article-body");
            var description = article.QuerySelector("div[itemprop='description']");
            var articleInner = doc.QuerySelector("div[itemprop='articleBody']");
            var builder = new StringBuilder();
            var pargraphs = articleInner.QuerySelectorAll("p");
            foreach (var pargraph in pargraphs)
            {
                foreach (var textNode in pargraph.ChildNodes.Where(item => item is HtmlTextNode || string.Compare(item.Name, "a", StringComparison.OrdinalIgnoreCase) == 0))
                {
                    builder.Append(textNode.InnerText.Trim());
                    builder.Append(' ');
                }
            }

            return new ArticleContent
            {
                Title = description.InnerText?.Trim(),
                Text = builder.ToString()
            };
        }
    }
}
