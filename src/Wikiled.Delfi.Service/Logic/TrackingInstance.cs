using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Monitoring;
using Wikiled.News.Monitoring.Persistency;

namespace Wikiled.Delfi.Service.Logic
{
    public class TrackingInstance : IArticlesPersistency, IHostedService
    {
        private readonly ILogger<TrackingInstance> logger;

        private readonly IArticlesPersistency persistency;

        private IArticlesMonitor articlesMonitor;

        private IDisposable initial;

        private IDisposable monitorArticles;

        public TrackingInstance(ILogger<TrackingInstance> logger, IArticlesPersistency persistency)
        {
            this.persistency = persistency ?? throw new ArgumentNullException(nameof(persistency));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting monitoring");
            initial = articlesMonitor
                          .NewArticlesStream()
                          .Select(Save)
                          .Merge()
                          .Subscribe();

            monitorArticles = articlesMonitor
                              .NewArticlesStream()
                                  .Select(Save)
                                  .Merge()
                                  .Subscribe();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            initial?.Dispose();
            monitorArticles?.Dispose();
            return Task.CompletedTask;
        }

        public async Task<bool> Save(Article article)
        {
            try
            {
                logger.LogDebug("Saving: {0} {1}", article.Definition.Feed.Category, article.Definition.Id);
                await Task.Run(() => persistency.Save(article)).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed processing");
            }

            return false;
        }
    }
}
