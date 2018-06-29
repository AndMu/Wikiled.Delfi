using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Wikiled.Delfi.Readers;

namespace Wikiled.Delfi.Tests.Readers
{
    [TestFixture]
    public class ArticleDataReaderTests
    {
        private Mock<ILoggerFactory> mockLoggerFactory;
        private Mock<ICommentsReaderFactory> mockCommentsReaderFactory;
        private Mock<IArticleTextReader> mockArticleTextReader;

        private ArticleDataReader instance;

        [SetUp]
        public void SetUp()
        {
            mockLoggerFactory = new Mock<ILoggerFactory>();
            mockCommentsReaderFactory = new Mock<ICommentsReaderFactory>();
            mockArticleTextReader = new Mock<IArticleTextReader>();
            instance = CreateInstance();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new ArticleDataReader(
                null,
                mockCommentsReaderFactory.Object,
                mockArticleTextReader.Object));
            Assert.Throws<ArgumentNullException>(() => new ArticleDataReader(
                mockLoggerFactory.Object,
                null,
                mockArticleTextReader.Object));
            Assert.Throws<ArgumentNullException>(() => new ArticleDataReader(
                mockLoggerFactory.Object,
                mockCommentsReaderFactory.Object,
                null));
        }

        private ArticleDataReader CreateInstance()
        {
            return new ArticleDataReader(
                mockLoggerFactory.Object,
                mockCommentsReaderFactory.Object,
                mockArticleTextReader.Object);
        }
    }
}
