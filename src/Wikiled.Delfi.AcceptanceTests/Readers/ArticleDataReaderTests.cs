using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Wikiled.Delfi.AcceptanceTests.Helper;
using Wikiled.Delfi.Containers;
using Wikiled.Delfi.Readers;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers;

namespace Wikiled.Delfi.AcceptanceTests.Readers
{
    [TestFixture]
    public class ArticleDataReaderTests
    {
        private IArticleDataReader instance;

        private NetworkHelper helper;

        [SetUp]
        public void SetUp()
        {
            helper = new NetworkHelper();
            instance = helper.Container.Resolve<IArticleDataReader>();
        }

        [TearDown]
        public void Teardown()
        {
            helper.Dispose();
        }

        [Test]
        public async Task ReadAll()
        {
            var tokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(2));
            var definition = new ArticleDefinition();
            definition.Url = new Uri("https://www.delfi.lt/auto/patarimai/siulo-keliuose-statyti-naujo-tipo-matuoklius-gales-daugiau-nei-isivaizduojate.d?id=80585701");
            var result = await instance.Read(definition, tokenSource.Token);
            Assert.IsNotNull(result);
            Assert.GreaterOrEqual(result.Content.Text.Length, 100);
            Assert.GreaterOrEqual(result.Content.Title.Length, 100);
            Assert.GreaterOrEqual(result.Comments.Length, 100);
        }
    }
}
