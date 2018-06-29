using System;
using Wikiled.Delfi.Feeds;

namespace Wikiled.Delfi.Data
{
    public class ArticleDefinition
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public DateTime? Date { get; set; }

        public string Topic { get; set; }

        public Uri Url { get; set; }

        public FeedName Feed { get; set; }

        public override string ToString()
        {
            return $"Article: {Id}";
        }
    }
}
