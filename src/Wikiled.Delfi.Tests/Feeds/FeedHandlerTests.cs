using System;
using NUnit.Framework;
using Wikiled.Delfi.Feeds;

namespace Wikiled.Delfi.Tests.Feeds
{
    [TestFixture]
    public class FeedHandlerTests
    {
        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new FeedsHandler(null));
        }
    }
}
