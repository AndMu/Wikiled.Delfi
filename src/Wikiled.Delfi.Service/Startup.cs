using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Autofac;
using Microsoft.AspNetCore.Builder;
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

namespace Wikiled.Delfi.Service
{
    public class Startup : BaseStartup
    {
        private readonly CompositeDisposable disposable = new CompositeDisposable();

        private readonly ILogger<Startup> logger;

        private MonitorConfig config;

        public Startup(ILoggerFactory loggerFactory, IHostingEnvironment env)
            : base(loggerFactory, env)
        {
            logger = loggerFactory.CreateLogger<Startup>();
        }

        public override void Configure(IApplicationBuilder app,
                                       IHostingEnvironment env,
                                       IApplicationLifetime applicationLifetime)
        {
            base.Configure(app, env, applicationLifetime);
            applicationLifetime.ApplicationStopping.Register(OnShutdown, disposable);
        }


        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            config = services.RegisterConfiguration<MonitorConfig>(Configuration.GetSection("Monitor"));
            config.Location.EnsureDirectoryExistence();
            return base.ConfigureServices(services);
        }

        private void OnShutdown(object toDispose)
        {
            ((IDisposable)toDispose).Dispose();
        }

        protected override void ConfigureSpecific(ContainerBuilder builder)
        {
            logger.LogDebug("ConfigureSpecific");
            builder.RegisterType<TrackingInstance>()
                .SingleInstance()
                .AutoActivate()
                .OnActivating(item =>
                {
                    logger.LogInformation("Starting monitoring");
                    var initial = item.Context.Resolve<IArticlesMonitor>()
                        .Start()
                        .Select(item.Instance.Save)
                        .Merge()
                        .Subscribe();
                    disposable.Add(initial);

                    var monitorArticles = item.Context.Resolve<IArticlesMonitor>()
                        .Monitor()
                        .Select(item.Instance.Save)
                        .Merge()
                        .Subscribe();
                    disposable.Add(monitorArticles);
                });

            builder.RegisterModule<MainModule>();
            var feeds = new FeedsFactory().Read().Result;
            builder.RegisterModule(DelfiModule.CreateWithFeeds(config.Location, feeds));
            builder.RegisterModule(new RetrieverModule(config.Service));
        }

        protected override string GetPersistencyLocation()
        {
            return config.Location;
        }
    }
}