﻿using System;
using System.Threading;
using Autofac;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Utilities.Config;
using Wikiled.Delfi.Readers;
using Wikiled.News.Monitoring.Feeds;
using Wikiled.News.Monitoring.Persistency;
using Wikiled.News.Monitoring.Readers;

namespace Wikiled.Delfi.Containers
{
    public class DelfiModule : Module
    {
        private readonly string saveLocation;

        public DelfiModule(string saveLocation)
        {
            this.saveLocation = saveLocation ?? throw new ArgumentNullException(nameof(saveLocation));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApplicationConfiguration>().As<IApplicationConfiguration>();
            builder.RegisterType<AlphaSessionReader>()
                .As<ISessionReader>()
                .SingleInstance()
                .OnActivating(async item => await item.Instance.Init(CancellationToken.None).ConfigureAwait(false));
            builder.RegisterType<CommentsReader>().As<ICommentsReader>();
            builder.RegisterType<ArticleTextReader>().As<IArticleTextReader>();
            //builder.RegisterType<AlphaDefinitionTransformer>().As<IDefinitionTransformer>();
            builder.Register(ctx => new ArticlesPersistency(ctx.Resolve<ILogger<ArticlesPersistency>>(), saveLocation)).As<IArticlesPersistency>();

            var feed = new FeedName();
            feed.Url = $"https://www.delfi.lt/rss/feeds/daily.xml";
            feed.Category = "Daily";
            builder.RegisterInstance(feed);
        }
    }
}
