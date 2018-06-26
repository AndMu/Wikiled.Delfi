using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Polly;

namespace Wikiled.Delfi.Articles
{
    public class CommentsReader : ICommentsReader
    {
        private readonly ILogger<CommentsReader> logger;

        private readonly Policy policy;

        private int pageSize = 20;

        private readonly ArticleDefinition article;

        private readonly IAdjuster adjuster;

        private bool isInit;

        public CommentsReader(ILogger<CommentsReader> logger, ArticleDefinition article, IAdjuster adjuster)
        {
            this.article = article ?? throw new ArgumentNullException(nameof(article));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.adjuster = adjuster ?? throw new ArgumentNullException(nameof(adjuster));
            policy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(new[] {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4)
                });
        }

        private string text;

        public int Total { get; private set; }

        public async Task Init()
        {
            text = await ReadAllText(0);
            var html = new HtmlDocument();
            html.LoadHtml(text);
            var doc = html.DocumentNode;
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
                    int totalPages = Total / pageSize;
                    if (Total % pageSize > 0)
                    {
                        totalPages++;
                    }

                    try
                    {
                        var tasks = new List<Task<string>>();
                        tasks.Add(Task.FromResult(text));
                        for (int i = 1; i < totalPages; i++)
                        {
                            tasks.Add(ReadAllText(i));
                        }

                        foreach (var task in tasks)
                        {
                            var result = await task;
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

        private Task<string> ReadAllText(int page)
        {
            HttpClient client = new HttpClient();
            return policy.ExecuteAsync(ct => client.GetStringAsync(GetUri(page)), CancellationToken.None);
        }

        private IEnumerable<CommentData> ParsePage(string page)
        {
            var html = new HtmlDocument();
            html.LoadHtml(page);
            var doc = html.DocumentNode;
            var comments = doc.QuerySelectorAll("div.comment-post");
            foreach (var htmlNode in comments)
            {
                CommentData record = new CommentData();
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
                record.UpVote = int.Parse(htmlNode.QuerySelector("div.comment-votes-up").InnerText.Trim());
                record.DownVote = int.Parse(htmlNode.QuerySelector("div.comment-votes-down").InnerText.Trim());
                var dateIp = htmlNode.QuerySelector("div.comment-date").InnerText.Trim();
                var ip = dateIp.IndexOf("IP:");
                record.Date = DateTime.Parse(dateIp.Substring(0, ip).Trim());
                record.Address = IPAddress.Parse(dateIp.Substring(ip + 3).Trim());
                yield return record;
            }
        }

        private string GetUri(int page)
        {
            var builder = new UriBuilder(article.Url);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["com"] = "1";
            adjuster.AddParametes(query);
            query["no"] = (page * pageSize).ToString();
            builder.Query = query.ToString();
            return builder.ToString();
        }
    }
}
