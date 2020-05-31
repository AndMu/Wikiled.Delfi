using System;
using System.Reactive.Disposables;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Extensions;
using Wikiled.Common.Utilities.Modules;
using Wikiled.Delfi.Containers;
using Wikiled.Delfi.Feeds;
using Wikiled.Delfi.Service.Config;
using Wikiled.Delfi.Service.Logic;
using Wikiled.News.Monitoring.Containers;
using Wikiled.News.Monitoring.Monitoring;
using Wikiled.Sentiment.Tracking.Service;
using Wikiled.Server.Core.Helpers;
using Wikiled.Server.Core.Performance;

namespace Wikiled.Delfi.Service
{
    public class Startup : BaseStartup
    {
        private readonly CompositeDisposable disposable = new CompositeDisposable();

        private readonly ILogger<Startup> logger;

        private MonitorConfig config;

        public Startup(ILoggerFactory loggerFactory, IWebHostEnvironment env)
            : base(loggerFactory, env)
        {
            logger = loggerFactory.CreateLogger<Startup>();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            config = services.RegisterConfiguration<MonitorConfig>(Configuration.GetSection("Monitor"));
            config.Location.EnsureDirectoryExistence();
            services.AddHostedService<ResourceMonitoringService>();
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            disposable.Dispose();
        }

        protected override void ConfigureSpecific(IServiceCollection builder)
        {
            logger.LogDebug("ConfigureSpecific");
            builder.AddHostedService<TrackingInstance>();
            builder.RegisterModule<MainNewsModule>();
            var feeds = new FeedsFactory().Read().Result;
            builder.RegisterModule(DelfiModule.CreateWithFeeds(config.Location, feeds));
            builder.RegisterModule(new NewsRetrieverModule(config.Service));
        }

        protected override string GetPersistencyLocation()
        {
            return config.Location;
        }
    }
}