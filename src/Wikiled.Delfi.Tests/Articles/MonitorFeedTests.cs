using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Wikiled.Delfi.Articles;

namespace Wikiled.Delfi.Tests.Articles
{
    [TestFixture]
    public class MonitorFeedTests
    {
        private MonitorFeed instance;

        [SetUp]
        public void SetUp()
        {
            instance = CreateInstance();
        }

        [Test]
        public async Task GetArticles()
        {
            var result = await instance.GetArticles().ToArray();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new MonitorFeed(null));
        }

        private MonitorFeed CreateInstance()
        {
            return new MonitorFeed(new[]{ "https://www.delfi.lt/rss/feeds/daily.xml" });
        }
    }
}
