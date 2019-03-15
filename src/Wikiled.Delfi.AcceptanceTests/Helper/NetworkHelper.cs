using Autofac;
using System;
using System.Net;
using Wikiled.Delfi.Containers;
using Wikiled.News.Monitoring.Containers;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.Delfi.AcceptanceTests.Helper
{
    public class NetworkHelper : IDisposable
    {
        public NetworkHelper()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<MainNewsModule>();
            builder.RegisterModule(DelfiModule.CreateDaily("Data"));

            builder.RegisterModule(
                new NewsRetrieverModule(
                    new RetrieveConfiguration
                    {
                        LongRetryDelay = 1000,
                        CallDelay = 0,
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

            Container = builder.Build();
            Retrieval = Container.Resolve<ITrackedRetrieval>();
        }

        public IContainer Container { get; }

        public ITrackedRetrieval Retrieval { get; }

        public void Dispose()
        {
            Container?.Dispose();
        }
    }
}
