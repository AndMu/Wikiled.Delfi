using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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
            var builder = new ServiceCollection();
            builder.RegisterModule<LoggingModule>();
            builder.RegisterModule<MainNewsModule>();
            builder.RegisterModule<CommonModule>();
            builder.RegisterModule(DelfiModule.CreateWithFeeds("Data", feeds));
            builder.RegisterModule(
                new NewsRetrieverModule(new RetrieveConfiguration
                {
                    LongDelay = 60 * 20,
                    CallDelay = 0,
                    ShortDelay = 1000,
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


            var container = builder.BuildServiceProvider();
            ILoggerFactory loggerFactory = container.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });

            IArticlesMonitor monitor = container.GetRequiredService<IArticlesMonitor>();
            "Data".EnsureDirectoryExistence();
            IArticlesPersistency persistency = container.GetRequiredService<IArticlesPersistency>();
            monitor.NewArticles().Subscribe(item => persistency.Save(item));
            monitor.MonitorUpdates().Subscribe(item => persistency.Save(item));
            Console.ReadLine();
        }
    }
}
