using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Wikiled.Delfi.Data;
using Wikiled.Delfi.Feeds;

namespace Wikiled.Delfi.Monitoring
{
    public class ArticlesMonitor : IArticlesMonitor
    {
        private readonly IFeedsHandler handler;

        private readonly IScheduler scheduler;

        private readonly ILogger<ArticlesMonitor> logger;

        private readonly IMemoryCache cache;

        public ArticlesMonitor(ILoggerFactory loggerFactory, IScheduler scheduler, IFeedsHandler handler, IMemoryCache cache)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            this.logger = loggerFactory.CreateLogger<ArticlesMonitor>();
            this.scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public IObservable<Article> Start()
        {
            logger.LogDebug("Start");
            return Observable.Interval(TimeSpan.FromHours(1), scheduler)
                .SelectMany(handler.GetArticles())
                .Select(ArticleReceived)
                .Where(item => item != null);
        }

        private Article ArticleReceived(ArticleDefinition article)
        {
            logger.LogDebug("ArticleReceived: {0}({1})", article.Topic, article.Id);
            if (cache.TryGetValue(article.ToString(), out var cached))
            {
                logger.LogDebug("Article already processed: {0}", article.Id);
                return null;
            }

            throw new NotImplementedException();
        }
    }
}
