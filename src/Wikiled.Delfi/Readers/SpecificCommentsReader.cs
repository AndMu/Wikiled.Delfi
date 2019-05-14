using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Delfi.Readers.Comments;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.Delfi.Readers
{
    public class SpecificCommentsReader : ISpecificCommentsReader
    {
        private readonly ArticleDefinition article;

        private readonly ILogger<SpecificCommentsReader> logger;

        private readonly ITrackedRetrieval reader;

        public SpecificCommentsReader(ILogger<SpecificCommentsReader> logger, ArticleDefinition article, ITrackedRetrieval reader)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.article = article ?? throw new ArgumentNullException(nameof(article));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public int Total { get; private set; }

        public IObservable<CommentData> ReadAllComments()
        {
            var all = Read(true, CancellationToken.None).ToObservable().SelectMany(item => GetComments(item, false));
            var anonym = Read(false, CancellationToken.None).ToObservable().SelectMany(item => GetComments(item, false));
            return all.Concat(anonym);
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
                        commentData.Positive = comment.Reaction.Where(item => item.Reaction == "Like").Sum(item => item.Count);
                        commentData.Negative = comment.Reaction.Where(item => item.Reaction == "Dislike").Sum(item => item.Count);
                    }
                }

                commentData.Text = comment?.Content;
                yield return commentData;
            }
        }

        private async Task<CommentsResult> Read(bool all, CancellationToken token)
        {
            var request = JsonConvert.SerializeObject(RequestComments.Create(long.Parse(article.Id), all));
            var json = await reader.Post(
                           new Uri("https://api.delfi.lt/comment/v1/graphql"),
                           request,
                           token,
                           webRequest =>
                           {
                               webRequest.Headers["Origin"] = "https://www.delfi.lt";
                               webRequest.Headers["Accept-Encoding"] = "gzip, deflate, br";
                               webRequest.Headers["Referer"] = article.Url.ToString();
                               webRequest.Headers["content-type"] = "application/json";
                           }).ConfigureAwait(false);
            var data = JsonConvert.DeserializeObject<CommentsResult>(json, Converter.Settings);
            return data;
        }
    }
}