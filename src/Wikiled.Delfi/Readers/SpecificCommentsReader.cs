using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Wikiled.Delfi.Adjusters;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.Delfi.Readers
{
    public class SpecificCommentsReader : ISpecificCommentsReader
    {
        private readonly IAdjuster adjuster;

        private readonly ArticleDefinition article;

        private readonly ILogger<SpecificCommentsReader> logger;

        private readonly ITrackedRetrieval reader;

        private HtmlDocument firstPage;

        private readonly int pageSize = 20;

        public SpecificCommentsReader(ILogger<SpecificCommentsReader> logger,
                                      ArticleDefinition article,
                                      IAdjuster adjuster,
                                      ITrackedRetrieval reader)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.article = article ?? throw new ArgumentNullException(nameof(article));
            this.adjuster = adjuster ?? throw new ArgumentNullException(nameof(adjuster));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public int Total { get; private set; }

        public IObservable<CommentData> ReadAllComments()
        {
            return Observable.Create<CommentData>(
                async observer =>
                {
                    await Init().ConfigureAwait(false);
                    var totalPages = Total / pageSize;
                    if (Total % pageSize > 0)
                    {
                        totalPages++;
                    }

                    try
                    {
                        var tasks = new List<Task<HtmlDocument>>();
                        tasks.Add(Task.FromResult(firstPage));
                        for (var i = 1; i < totalPages; i++)
                        {
                            tasks.Add(GetDocumentById(i));
                        }

                        foreach (var task in tasks)
                        {
                            var result = await task.ConfigureAwait(false);
                            foreach (var commentData in ParsePage(result))
                            {
                                observer.OnNext(commentData);
                            }
                        }

                        observer.OnCompleted();
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                });
        }

        private async Task<HtmlDocument> GetDocumentById(int id)
        {
            return (await reader.Read(GetUri(id), CancellationToken.None).ConfigureAwait(false)).GetDocument();
        }

        private IEnumerable<CommentData> ParsePage(HtmlDocument html)
        {
            var doc = html.DocumentNode;
            var comments = doc.QuerySelectorAll("div.comment-post");
            foreach (var htmlNode in comments)
            {
                var record = new CommentData();
                var author = htmlNode.QuerySelector("div.comment-author");
                if (author == null)
                {
                    logger.LogDebug("Author not found: {0}", htmlNode.InnerText.Trim());
                    continue;
                }

                record.Id = htmlNode.Attributes["data-post-id"].Value;
                record.Author = author.InnerText.Trim();
                var comment = htmlNode.QuerySelector("div.comment-content-inner");
                if (comment == null)
                {
                    logger.LogDebug("Comment not found for {0}[{1}]", record.Id, record.Author);
                    continue;
                }

                record.Text = comment.InnerText.Trim();
                var upVote = htmlNode.QuerySelector("div.comment-votes-up")?.InnerText;
                if (!string.IsNullOrEmpty(upVote))
                {
                    record.Positive = int.Parse(upVote.Trim());
                }

                var downVote = htmlNode.QuerySelector("div.comment-votes-down")?.InnerText;
                if (!string.IsNullOrEmpty(downVote))
                {
                    record.Negative = int.Parse(downVote.Trim());
                }

                var dateIp = htmlNode.QuerySelector("div.comment-date").InnerText.Trim();
                var ip = dateIp.IndexOf("IP:");
                record.Date = DateTime.Parse(ip == -1 ? dateIp.Trim() : dateIp.Substring(0, ip).Trim());
                yield return adjuster.Transform(record);
            }
        }

        private Uri GetUri(int page)
        {
            var builder = new UriBuilder(article.Url);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["com"] = "1";
            adjuster.AddParametres(query);
            query["no"] = (page * pageSize).ToString();
            builder.Query = query.ToString();
            return new Uri(builder.ToString());
        }

        private async Task Init()
        {
            firstPage = (await reader.Read(GetUri(0), CancellationToken.None).ConfigureAwait(false)).GetDocument();
            var doc = firstPage.DocumentNode;
            var commentsDefinition = doc.QuerySelector("div#comments-list");
            Total = int.Parse(commentsDefinition.Attributes.First(a => a.Name == "data-count").Value);
        }
    }
}