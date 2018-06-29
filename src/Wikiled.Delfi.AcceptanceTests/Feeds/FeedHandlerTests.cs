using System.Reactive.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Delfi.Feeds;

namespace Wikiled.Delfi.AcceptanceTests.Feeds
{
    [TestFixture]
    public class FeedHandlerTests
    {
        private FeedsHandler instance;

        [SetUp]
        public void SetUp()
        {
            instance = CreateInstance();
        }

        [Test]
        public async Task GetArticles()
        {
            var result = await instance.GetArticles().ToArray();
            Assert.Greater(result.Length, 0);
        }

        private FeedsHandler CreateInstance()
        {
            FeedName feed = new FeedName();
            feed.Url = "https://www.delfi.lt/rss/feeds/daily.xml";
            feed.Category = "Daily";
            return new FeedsHandler(new[] { feed });
        }
    }
}
