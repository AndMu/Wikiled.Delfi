using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Web;
using CodeHollow.FeedReader;
using Wikiled.Delfi.Data;

namespace Wikiled.Delfi.Feeds
{
    public class FeedsHandler : IFeedsHandler
    {
        private readonly FeedName[] feeds;

        public FeedsHandler(FeedName[] feeds)
        {
            this.feeds = feeds ?? throw new ArgumentNullException(nameof(feeds));
        }

        public IObservable<ArticleDefinition> GetArticles()
        {
            return Observable.Create<ArticleDefinition>(
                async observer =>
                {
                    List<(FeedName Feed, Task<Feed> Task)> tasks = new List<(FeedName Feed, Task<Feed> Task)>();
                    foreach (var feed in feeds)
                    {
                        var task = FeedReader.ReadAsync(feed.Url);
                        tasks.Add((feed, task));
                    }

                    foreach (var task in tasks)
                    {
                        var result = await task.Task;
                        foreach (var item in result.Items)
                        {
                            ArticleDefinition article = new ArticleDefinition();
                            article.Url = new Uri(item.Link);
                            UriBuilder builder = new UriBuilder(article.Url);
                            var query = HttpUtility.ParseQueryString(builder.Query);
                            article.Id = query["id"];
                            article.Date = item.PublishingDate;
                            article.Title = item.Title;
                            article.Feed = task.Feed;
                            observer.OnNext(article);
                        }
                    }

                    observer.OnCompleted();
                });
        }
    }
}
