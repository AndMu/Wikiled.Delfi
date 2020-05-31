using System;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Wikiled.Common.Utilities.Modules;
using Wikiled.Delfi.Containers;
using Wikiled.News.Monitoring.Containers;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.Delfi.AcceptanceTests.Helper
{
    public class NetworkHelper : IDisposable
    {
        public NetworkHelper()
        {
            var builder = new ServiceCollection();
            builder.RegisterModule<MainNewsModule>();
            builder.RegisterModule<LoggingModule>();
            builder.RegisterModule(DelfiModule.CreateDaily("Data"));

            builder.RegisterModule(
                new NewsRetrieverModule(
                    new RetrieveConfiguration
                    {
                        LongDelay = 1000,
                        CallDelay = 0,
                        ShortDelay = 1000,
                        LongRetryCodes = new[] { HttpStatusCode.Forbidden },
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

            Container = builder.BuildServiceProvider();
            Retrieval = Container.GetRequiredService<ITrackedRetrieval>();
        }

        public ServiceProvider Container { get; }

        public ITrackedRetrieval Retrieval { get; }

        public void Dispose()
        {
            Container?.Dispose();
        }
    }
}
