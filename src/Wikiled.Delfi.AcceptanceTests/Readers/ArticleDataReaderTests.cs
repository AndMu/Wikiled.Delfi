using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Wikiled.Delfi.AcceptanceTests.Helper;
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
            instance = helper.Container.GetRequiredService<IArticleDataReader>();
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
            definition.Id = "80585701";
            definition.Url = new Uri("https://www.delfi.lt/auto/patarimai/siulo-keliuose-statyti-naujo-tipo-matuoklius-gales-daugiau-nei-isivaizduojate.d?id=80585701");
            var result = await instance.Read(definition, tokenSource.Token).ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.GreaterOrEqual(result.Content.Text.Length, 100);
            Assert.GreaterOrEqual(result.Content.Title.Length, 50);
            Assert.GreaterOrEqual(result.Comments.Length, 100);
        }
    }
}
