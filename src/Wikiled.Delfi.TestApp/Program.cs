﻿using Autofac;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using CodeHollow.FeedReader;
using Wikiled.Common.Extensions;
using Wikiled.Common.Utilities.Modules;
using Wikiled.Delfi.Containers;
using Wikiled.Delfi.Feeds;
using Wikiled.News.Monitoring.Containers;
using Wikiled.News.Monitoring.Monitoring;
using Wikiled.News.Monitoring.Persistency;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.Delfi.TestApp
{
    public class Program
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static async Task Main(string[] args)
        {
            log.Info("Starting {0} version utility...", Assembly.GetExecutingAssembly().GetName().Version);
            var feeds = await new FeedsFactory().Read().ConfigureAwait(false);
            var builder = new ContainerBuilder();
            builder.RegisterModule<MainModule>();
            builder.RegisterModule<CommonModule>();
            builder.RegisterModule(DelfiModule.CreateWithFeeds("Data", feeds));
            builder.RegisterModule(
                new RetrieverModule(new RetrieveConfiguration
                {
                    LongRetryDelay = 60 * 20,
                    CallDelay = 0,
                    LongRetryCodes = new[] { HttpStatusCode.Forbidden, },
                    RetryCodes = new[]
                    {
                        HttpStatusCode.Forbidden,
                        HttpStatusCode.RequestTimeout, // 408
                        HttpStatusCode.InternalServerError, // 500
                        HttpStatusCode.BadGateway, // 502
                        HttpStatusCode.ServiceUnavailable, // 503
                        HttpStatusCode.GatewayTimeout // 504
                    },
                    MaxConcurrent = 1
                }));


            IContainer container = builder.Build();
            ILoggerFactory loggerFactory = container.Resolve<ILoggerFactory>();
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });

            IArticlesMonitor monitor = container.Resolve<IArticlesMonitor>();
            "Data".EnsureDirectoryExistence();
            IArticlesPersistency persistency = container.Resolve<IArticlesPersistency>();
            monitor.Start().Subscribe(item => persistency.Save(item));
            monitor.Monitor().Subscribe(item => persistency.Save(item));
            Console.ReadLine();
        }
    }
}