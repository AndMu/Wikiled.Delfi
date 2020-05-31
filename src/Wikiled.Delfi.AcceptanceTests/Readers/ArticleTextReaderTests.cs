using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Wikiled.Delfi.AcceptanceTests.Helper;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.Delfi.AcceptanceTests.Readers
{
    [TestFixture]
    public class ArticleTextReaderTests
    {
        private IArticleTextReader instance;

        private NetworkHelper helper;

        [SetUp]
        public void SetUp()
        {
            helper = new NetworkHelper();
            instance = helper.Container.GetRequiredService<Func<ITrackedRetrieval, IArticleTextReader>>()(helper.Container.GetRequiredService<ITrackedRetrieval>());
        }

        [TearDown]
        public void Teardown()
        {
            helper.Dispose();
        }

        [Test]
        public async Task ReadArticle()
        {
            var tokenSource = new CancellationTokenSource(2000);
            var definition = new ArticleDefinition();
            definition.Url = new Uri("https://www.delfi.lt/auto/patarimai/siulo-keliuose-statyti-naujo-tipo-matuoklius-gales-daugiau-nei-isivaizduojate.d?id=80585701");
            var text = await instance.ReadArticle(definition, tokenSource.Token);
            Assert.IsNotNull(text);
            Assert.GreaterOrEqual(text.Text.Length, 100);
            Assert.GreaterOrEqual(text.Title.Length, 10);
        }
    }
}
