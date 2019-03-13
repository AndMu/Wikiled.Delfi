using System;
using System.Threading;
using Autofac;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Utilities.Config;
using Wikiled.Delfi.Adjusters;
using Wikiled.Delfi.Readers;
using Wikiled.News.Monitoring.Feeds;
using Wikiled.News.Monitoring.Persistency;
using Wikiled.News.Monitoring.Readers;

namespace Wikiled.Delfi.Containers
{
    public class DelfiModule : Module
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

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApplicationConfiguration>().As<IApplicationConfiguration>();
            builder.RegisterType<AnonymousAdjuster>().As<IAdjuster>();
            builder.RegisterType<RegisteredAdjuster>().As<IAdjuster>();
            builder.RegisterType<SpecificCommentsReader>().As<ISpecificCommentsReader>();
            builder.RegisterType<CommentsReader>().As<ICommentsReader>();
            builder.RegisterType<ArticleTextReader>().As<IArticleTextReader>();
            builder.RegisterType<NullAuthentication>().As<IAuthentication>();
            builder.RegisterType<DelfiDefinitionTransformer>().As<IDefinitionTransformer>();
            builder.Register(ctx => new ArticlesPersistency(ctx.Resolve<ILogger<ArticlesPersistency>>(), saveLocation))
                .As<IArticlesPersistency>();
            foreach (var feed in feeds)
            {
                builder.RegisterInstance(feed);
            }
        }
    }
}
