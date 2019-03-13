using CodeHollow.FeedReader;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Wikiled.News.Monitoring.Feeds;

namespace Wikiled.Delfi.Feeds
{
    public class FeedsFactory
    {
        public async Task<FeedName[]> Read()
        {
            var result = await FeedReader.ReadAsync("https://www.delfi.lt/rss/feeds/index.xml").ConfigureAwait(false);
            var feeds = new List<FeedName>();
            foreach (var item in result.Items)
            {
                var feed = new FeedName();
                feed.Category = Path.GetFileNameWithoutExtension(item.Link);
                feed.Url = item.Link;
                feeds.Add(feed);
            }

            return feeds.ToArray();
        }
    }
}
