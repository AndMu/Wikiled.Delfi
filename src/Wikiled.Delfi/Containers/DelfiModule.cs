using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;
using Wikiled.Common.Utilities.Config;
using Wikiled.Common.Utilities.Modules;
using Wikiled.Delfi.Readers;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Feeds;
using Wikiled.News.Monitoring.Persistency;
using Wikiled.News.Monitoring.Readers;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.Delfi.Containers
{
    public class DelfiModule : IModule
    {
        private readonly string saveLocation;

        private readonly FeedName[] feeds;

        private DelfiModule(string saveLocation, params FeedName[] feeds)
        {
            this.saveLocation = saveLocation ?? throw new ArgumentNullException(nameof(saveLocation));
            this.feeds = feeds ?? throw new ArgumentNullException(nameof(feeds));
        }

        public static DelfiModule CreateWithFeeds(string saveLocation, FeedName[] feed)
        {
            return new DelfiModule(saveLocation, feed);
        }

        public static DelfiModule CreateDaily(string saveLocation)
        {
            var feed = new FeedName();
            feed.Url = "https://www.delfi.lt/rss/feeds/daily.xml";
            feed.Category = "Daily";
            return new DelfiModule(saveLocation, feed);
        }

        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IApplicationConfiguration, ApplicationConfiguration>();
            services.AddSingleton(
                ctx => (Func<ITrackedRetrieval, ArticleDefinition, ICommentsReader>)((arg, def) => new CommentsReader(ctx.GetRequiredService<ILogger<CommentsReader>>(), def, arg)));
            services.AddSingleton(
                ctx => (Func<ITrackedRetrieval, IArticleTextReader>)(arg => new ArticleTextReader(ctx.GetRequiredService<ILogger<ArticleTextReader>>(), arg)));
            services.AddSingleton(
                ctx => (Func<ITrackedRetrieval, IAuthentication>)(arg => new NullAuthentication(arg)));
            services.AddTransient<IDefinitionTransformer, DelfiDefinitionTransformer>();

            services.AddTransient<IArticlesPersistency>(ctx => new ArticlesPersistency(ctx.GetRequiredService<ILogger<ArticlesPersistency>>(), saveLocation));
            services.AddSingleton(feeds);

            return services;
        }
    }
}
