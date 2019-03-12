using NLog;
using NLog.Extensions.Logging;
using NUnit.Framework;
using Wikiled.Common.Logging;

namespace Wikiled.Delfi.AcceptanceTests
{
    [SetUpFixture]
    public class Global
    {
        [OneTimeSetUp]
        public void Setup()
        {
            LogManager.LoadConfiguration("nlog.config");
            ApplicationLogging.LoggerFactory.AddNLog();
            var log = LogManager.GetCurrentClassLogger();
            log.Info("Starting");
        }
    }
}
