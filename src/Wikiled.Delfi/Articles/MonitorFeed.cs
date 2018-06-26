using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CodeHollow.FeedReader;

namespace Wikiled.Delfi.Articles
{
    public class MonitorFeed
    {
        private readonly string[] feeds;

        public MonitorFeed(string[] feeds)
        {
            this.feeds = feeds ?? throw new ArgumentNullException(nameof(feeds));
        }

        public IObservable<ArticleDefinition> GetArticles()
        {
            return Observable.Create<ArticleDefinition>(
                async observer =>
                {
                    List<Task<Feed>> tasks = new List<Task<Feed>>();
                    foreach (var feed in feeds)
                    {
                        var task = FeedReader.ReadAsync(feed);
                        tasks.Add(task);
                    }

                    foreach (var task in tasks)
                    {
                        var result = await task;
                        foreach (var item in result.Items)
                        {
                            ArticleDefinition article = new ArticleDefinition();
                            article.Url = new Uri(item.Link);
                            article.Date = item.PublishingDate;
                            article.Title = item.Title;
                            article.
                        }
                    }
                });
        }

    }
}
