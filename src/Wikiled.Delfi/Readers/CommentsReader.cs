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
using Wikiled.News.Monitoring.Readers;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.Delfi.Readers
{
    public class CommentsReader : ICommentsReader
    {
        private readonly ILogger<CommentsReader> logger;

        private int pageSize = 20;

        private readonly ArticleDefinition article;

        private readonly IAdjuster adjuster;

        private bool isInit;

        private readonly ITrackedRetrieval reader;


        public CommentsReader(ILoggerFactory loggerFactory, ArticleDefinition article, IAdjuster adjuster, ITrackedRetrieval reader)
        {
            this.article = article ?? throw new ArgumentNullException(nameof(article));
            logger = loggerFactory?.CreateLogger<CommentsReader>() ?? throw new ArgumentNullException(nameof(logger));
            this.adjuster = adjuster ?? throw new ArgumentNullException(nameof(adjuster));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        private HtmlDocument firstPage;

        public int Total { get; private set; }

        public async Task Init()
        {
            firstPage = (await reader.Read(GetUri(0), CancellationToken.None).ConfigureAwait(false)).GetDocument();
            var doc = firstPage.DocumentNode;
            var commentsDefinition = doc.QuerySelector("div#comments-list");
            Total = int.Parse(commentsDefinition.Attributes.First(a => a.Name == "data-count").Value);
            isInit = true;
        }

        public IObservable<CommentData> ReadAllComments()
        {
            if (!isInit)
            {
                throw new InvalidOperationException("Reader is not initialized");
            }

            if (article == null)
            {
                throw new ArgumentNullException(nameof(article));
            }

            return Observable.Create<CommentData>(
                async observer =>
                {
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
                var upVote = int.Parse(htmlNode.QuerySelector("div.comment-votes-up").InnerText.Trim());
                var downVote = int.Parse(htmlNode.QuerySelector("div.comment-votes-down").InnerText.Trim());
                //record.Vote = 
                var dateIp = htmlNode.QuerySelector("div.comment-date").InnerText.Trim();
                var ip = dateIp.IndexOf("IP:");
                if (ip == -1)
                {
                    record.Date = DateTime.Parse(dateIp.Trim());
                }
                else
                {
                    record.Date = DateTime.Parse(dateIp.Substring(0, ip).Trim());
                }

                yield return record;
            }
        }

        private Uri GetUri(int page)
        {
            var builder = new UriBuilder(article.Url);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["com"] = "1";
            adjuster.AddParametes(query);
            query["no"] = (page * pageSize).ToString();
            builder.Query = query.ToString();
            return new Uri(builder.ToString());
        }
    }
}
