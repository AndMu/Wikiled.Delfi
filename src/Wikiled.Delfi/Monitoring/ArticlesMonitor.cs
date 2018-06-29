using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Utilities.Config;
using Wikiled.Delfi.Data;
using Wikiled.Delfi.Feeds;
using Wikiled.Delfi.Readers;

namespace Wikiled.Delfi.Monitoring
{
    public class ArticlesMonitor : IArticlesMonitor
    {
        private readonly IApplicationConfiguration configuration;

        private readonly IFeedsHandler handler;

        private readonly IScheduler scheduler;

        private readonly ILogger<ArticlesMonitor> logger;

        private readonly IArticleDataReader articleDataReader;

        private readonly ConcurrentDictionary<string, Article> scanned = new ConcurrentDictionary<string, Article>(StringComparer.OrdinalIgnoreCase);

        public ArticlesMonitor(IApplicationConfiguration configuration, ILoggerFactory loggerFactory, IScheduler scheduler, IFeedsHandler handler, IArticleDataReader articleDataReader)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            logger = loggerFactory.CreateLogger<ArticlesMonitor>();
            this.scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
            this.articleDataReader = articleDataReader;
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IObservable<Article> Start()
        {
            logger.LogDebug("Start");
            var scanFeed = Observable.Interval(TimeSpan.FromHours(1), scheduler)
                                     .StartWith(0)
                                     .SelectMany(handler.GetArticles())
                                     .Where(item => !scanned.ContainsKey(item.Id))
                                     .Select(ArticleReceived)
                                     .Merge()
                                     .Where(item => item != null);

            var updated = Observable.Interval(TimeSpan.FromHours(6), scheduler).Select(item => Updated()).Merge();
            return scanFeed.Merge(updated);
        }

        private IObservable<Article> Updated()
        {
            return Observable.Create<Article>(
                async observer =>
                {
                    logger.LogInformation("Checking requirement to refresh");
                    List<(Article Article, Task<bool> Task)> refreshTask = new List<(Article, Task<bool>)>();
                    var expire = configuration.Now.AddDays(-5);
                    foreach (var article in scanned.ToArray())
                    {
                        if (article.Value.DateTime < expire)
                        {
                            scanned.TryRemove(article.Key, out _);
                            continue;
                        }
                        
                        refreshTask.Add((article.Value, articleDataReader.RequiresRefreshing(article.Value)));
                    }

                    logger.LogInformation("Waiting {0} tasks on refresh pre-check to complete", refreshTask.Count);
                    await Task.WhenAll(refreshTask.Select(item => item.Task)).ConfigureAwait(false);
                    List<Task<Article>> refreshTaskFinal = new List<Task<Article>>();
                    foreach (var refresh in refreshTask.Where(item => item.Task.Result))
                    {
                        refreshTaskFinal.Add(ArticleReceived(refresh.Article.Definition));
                    }

                    foreach (var task in refreshTaskFinal)
                    {
                        var result = await task.ConfigureAwait(false);
                        observer.OnNext(result);
                    }

                    logger.LogInformation("Refresh completed"); 
                    observer.OnCompleted();
                });
        }

        private async Task<Article> ArticleReceived(ArticleDefinition article)
        {
            logger.LogDebug("ArticleReceived: {0}({1})", article.Topic, article.Id);
            var result = await articleDataReader.Read(article).ConfigureAwait(false); 
            scanned[result.Definition.Id] = result;
            return result;
        }
    }
}
