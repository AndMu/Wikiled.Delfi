using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Persistency;

namespace Wikiled.Delfi.Service.Logic
{
    public class TrackingInstance : IArticlesPersistency
    {
        private readonly ILogger<TrackingInstance> logger;

        private readonly IArticlesPersistency persistency;

        public TrackingInstance(ILogger<TrackingInstance> logger, IArticlesPersistency persistency)
        {
            this.persistency = persistency ?? throw new ArgumentNullException(nameof(persistency));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
