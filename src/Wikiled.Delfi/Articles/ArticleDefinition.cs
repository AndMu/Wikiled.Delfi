using System;

namespace Wikiled.Delfi.Articles
{
    public class ArticleDefinition
    {
        public string Title { get; set; }

        public DateTime Date { get; set; }

        public string Topic { get; set; }

        public Uri Url { get; set; }

        public string Text { get; set; }
    }
}
