using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    public class ArticleEntity
    {
        [JsonProperty("article_id")]
        public long ArticleId { get; set; }

        [JsonProperty("count_total")]
        public long CountTotal { get; set; }

        [JsonProperty("count_total_main_posts")]
        public long CountTotalMainPosts { get; set; }

        [JsonProperty("count_registered")]
        public long CountRegistered { get; set; }

        [JsonProperty("count_registered_main_posts")]
        public long CountRegisteredMainPosts { get; set; }

        [JsonProperty("count_anonymous_main_posts")]
        public long CountAnonymousMainPosts { get; set; }

        [JsonProperty("count_anonymous")]
        public long CountAnonymous { get; set; }

        [JsonProperty("comments")]
        public Comment[] Comments { get; set; }

        [JsonProperty("__typename")]
        public string Typename { get; set; }
    }
}
