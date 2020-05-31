using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Extensions;
using Wikiled.News.Monitoring.Readers;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.Delfi.Readers
{
    public class ArticleTextReader : IArticleTextReader
    {
        private readonly ILogger<ArticleTextReader> logger;

        private readonly ITrackedRetrieval reader;

        public ArticleTextReader(ILogger<ArticleTextReader> logger, ITrackedRetrieval reader)
        {
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ArticleContent> ReadArticle(ArticleDefinition definition, CancellationToken token)
        {
            logger.LogDebug("Reading article text: {0}", definition.Id);
            var page = (await reader.Read(definition.Url, token).ConfigureAwait(false)).GetDocument();
            var doc = page.DocumentNode;
            var article = doc.QuerySelector("div.article-body");
            var description = doc.QuerySelector("div.article-title h1");
            var lead = article.QuerySelector("div.delfi-article-lead");
            var row = article.QuerySelector("div.col-xs-8");

            var builder = new StringBuilder();
            var pargraphs = lead.QuerySelectorAll("p")
                                .Concat(row.ChildNodes.First(item => string.Compare(item.Name, "DIV", StringComparison.OrdinalIgnoreCase) == 0)
                                           .QuerySelectorAll("p"));
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
