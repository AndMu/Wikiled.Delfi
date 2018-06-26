using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Wikiled.Delfi.Articles.Feeds;

namespace Wikiled.Delfi.Articles
{
    public class ArticlesMonitor : IDisposable
    {
        private readonly IFeedsHandler handler;

        private readonly IScheduler scheduler;

        private readonly ILogger<ArticlesMonitor> logger;

        private IMemoryCache cache;

        private IDisposable subscription;

        public ArticlesMonitor(ILoggerFactory loggerFactory, IScheduler scheduler, IFeedsHandler handler, IMemoryCache cache)
        {
            this.logger = loggerFactory.CreateLogger<ArticlesMonitor>();
            this.scheduler = scheduler;
            this.handler = handler;
            this.cache = cache;
        }

        public void Start()
        {
            logger.LogDebug("Start");
            subscription = Observable.Interval(TimeSpan.FromHours(1), scheduler).SelectMany(handler.GetArticles()).Subscribe(ArticleReceived);
        }

        private void ArticleReceived(ArticleDefinition article)
        {
            logger.LogDebug("ArticleReceived: {0}", article.Topic);
        }

        public void Dispose()
        {
            logger.LogDebug("Dispose");
            subscription?.Dispose();
        }
    }
}
