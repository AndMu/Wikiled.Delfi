using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Wikiled.Delfi.Data;
using Wikiled.Delfi.Readers;

namespace Wikiled.Delfi.AcceptanceTests.Readers
{
    [TestFixture]
    public class ArticleDataReaderTests
    {
        private ArticleDataReader instance;

        [SetUp]
        public void SetUp()
        {
            instance = CreateInstance();
        }

        [Test]
        public async Task ReadAll()
        {
            ArticleDefinition definition = new ArticleDefinition();
            definition.Url = new Uri("https://www.delfi.lt/veidai/zmones/petro-grazulio-dukreles-mama-paviesino-ju-susirasinejima-galite-suprasti-kaip-jauciausi.d?id=78390475");
            var result = await instance.Read(definition);
            Assert.IsNotNull(result);
            Assert.GreaterOrEqual(result.ArticleText.Text.Length, 100);
            Assert.GreaterOrEqual(result.ArticleText.Description.Length, 100);
            Assert.GreaterOrEqual(result.Anonymous.Comments.Length, 1000);
        }

     

        private ArticleDataReader CreateInstance()
        {
            return new ArticleDataReader(
                new NullLoggerFactory(),
                new CommentsReaderFactory(new NullLoggerFactory(), new HtmlReader()),
                new ArticleTextReader(new NullLoggerFactory(), new HtmlReader()));
        }
    }
}
