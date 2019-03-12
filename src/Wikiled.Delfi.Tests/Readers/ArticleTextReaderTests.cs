using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NUnit.Framework.Internal;
using Wikiled.Delfi.Readers;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Retriever;

namespace Wikiled.Delfi.Tests.Readers
{
    [TestFixture]
    public class ArticleTextReaderTests
    {
        private Mock<ITrackedRetrieval> retriever;

        private ArticleTextReader instance;

        [SetUp]
        public void SetUp()
        {
            retriever = new Mock<ITrackedRetrieval>();
            retriever.Setup(item => item.Read(It.IsAny<Uri>(),
                                              It.IsAny<CancellationToken>(),
                                              It.IsAny<Action<System.Net.HttpWebRequest>>()))
                .Returns(Task.FromResult(
                             File.ReadAllText(
                                 Path.Combine(TestContext.CurrentContext.TestDirectory, @"Data\data.html"))));
            instance = CreateInstance();
        }

        [Test]
        public async Task ReadArticle()
        {
            var definition = new ArticleDefinition();
            definition.Url = new Uri("https://www.delfi.lt/zz");
            var text = await instance.ReadArticle(definition, CancellationToken.None);
            Assert.IsNotNull(text);
            Assert.GreaterOrEqual(text.Text.Length, 100);
            Assert.GreaterOrEqual(text.Title.Length, 100);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new ArticleTextReader(null, retriever.Object));
            Assert.Throws<ArgumentNullException>(() => new ArticleTextReader(new NullLoggerFactory(),  null));
        }

        private ArticleTextReader CreateInstance()
        {
            return new ArticleTextReader(new NullLoggerFactory(), retriever.Object);
        }
    }
}
