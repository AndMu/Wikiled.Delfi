using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Delfi.Readers.Comments;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.Delfi.Readers
{
    public class CommentsReader : ICommentsReader
    {
        private readonly ArticleDefinition article;

        private const int pageSize = 20;

        private readonly ILogger<CommentsReader> logger;

        private readonly ITrackedRetrieval reader;

        public CommentsReader(ILogger<CommentsReader> logger, ArticleDefinition article, ITrackedRetrieval reader)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.article = article ?? throw new ArgumentNullException(nameof(article));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public int Total { get; private set; }

        public IObservable<CommentData> ReadAllComments()
        {
            return Observable.Create<CommentData>(
                async observer =>
                {
                    var result = await Read(true, 0, CancellationToken.None).ConfigureAwait(false);
                    if (result.Data.GetCommentsByConfig.ArticleEntity == null)
                    {
                        logger.LogInformation("No comments found for {0}", article.Id);
                        observer.OnCompleted();
                        return;
                    }

                    var registered = CountPages(result.Data.GetCommentsByConfig.ArticleEntity.CountRegistered);
                    var anonymous = CountPages(result.Data.GetCommentsByConfig.ArticleEntity.CountAnonymous);
                    Total = result.Data.GetCommentsByConfig.ArticleEntity.CountTotal;
                    foreach (var comment in GetComments(result, true))
                    {
                        observer.OnNext(comment);
                    }

                    logger.LogDebug("Found {0} ({1}/{2}) comments for {3} article. Total pages {4}/{5}",
                                    result.Data.GetCommentsByConfig.ArticleEntity.CountTotal,
                                    result.Data.GetCommentsByConfig.ArticleEntity.CountRegistered,
                                    result.Data.GetCommentsByConfig.ArticleEntity.CountAnonymous,
                                    article.Id,
                                    registered,
                                    anonymous);

                    var tasks = new List<(bool Type, Task<CommentsResult> Task)>();
                    for (int i = 1; i < registered; i++)
                    {
                        var task = Read(true, i, CancellationToken.None);
                        tasks.Add((true, task));
                    }

                    for (int i = 0; i < anonymous; i++)
                    {
                        var task = Read(false, i, CancellationToken.None);
                        tasks.Add((false, task));
                    }

                    foreach (var task in tasks)
                    {
                        var taskResult = await task.Task.ConfigureAwait(false);
                        foreach (var comment in GetComments(taskResult, task.Type))
                        {
                            observer.OnNext(comment);
                        }
                    }

                    observer.OnCompleted();
                });
        }

        private int CountPages(int total)
        {
            var totalPages = total / pageSize;
            if (total % pageSize > 0)
            {
                totalPages++;
            }

            return totalPages;
        }

        private IEnumerable<CommentData> GetComments(CommentsResult result, bool registered)
        {
            Total = result.Data.GetCommentsByConfig.ArticleEntity.CountTotal;
            foreach (var comment in result.Data.GetCommentsByConfig.ArticleEntity.Comments)
            {
                var commentData = new CommentData();
                commentData.AdditionalData = comment;
                commentData.Id = comment?.Id.ToString();
                commentData.Author = comment?.Author?.DisplayName;
                commentData.AuthorId = comment?.Author?.Id.ToString();
                commentData.IsSpecialAuthor = registered;
                if (comment != null)
                {
                    commentData.Date = comment.CreatedTime.DateTime;
                    if (comment.Reaction != null)
                    {
                        commentData.Positive = comment.Reaction.Where(item => string.Compare(item.Reaction, "Like", StringComparison.OrdinalIgnoreCase) == 0).Sum(item => item.Count);
                        commentData.Negative = comment.Reaction.Where(item => string.Compare(item.Reaction, "Dislike", StringComparison.OrdinalIgnoreCase) == 0).Sum(item => item.Count);
                    }
                }

                commentData.Text = comment?.Content;
                yield return commentData;
            }
        }

        private async Task<CommentsResult> Read(bool all, int page, CancellationToken token)
        {
            var request = JsonConvert.SerializeObject(RequestComments.Create(long.Parse(article.Id), all, page: page));
            var json = await reader.Post(
                           new Uri("https://api.delfi.lt/comment/v1/graphql"),
                           request,
                           token,
                           webRequest =>
                           {
                               webRequest.Headers["Origin"] = "https://www.delfi.lt";
                               webRequest.Headers["Accept-Encoding"] = "gzip, deflate, br";
                               webRequest.Referer = article.Url.ToString();
                               webRequest.ContentType = "application/json";
                           }).ConfigureAwait(false);
            var data = JsonConvert.DeserializeObject<CommentsResult>(json, Converter.Settings);
            return data;
        }
    }
}