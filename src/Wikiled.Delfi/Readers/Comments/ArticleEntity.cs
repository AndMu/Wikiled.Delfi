using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    public class ArticleEntity
    {
        [JsonProperty("article_id")]
        public int ArticleId { get; set; }

        [JsonProperty("count_total")]
        public int CountTotal { get; set; }

        [JsonProperty("count_total_main_posts")]
        public int CountTotalMainPosts { get; set; }

        [JsonProperty("count_registered")]
        public int CountRegistered { get; set; }

        [JsonProperty("count_registered_main_posts")]
        public int CountRegisteredMainPosts { get; set; }

        [JsonProperty("count_anonymous_main_posts")]
        public int CountAnonymousMainPosts { get; set; }

        [JsonProperty("count_anonymous")]
        public int CountAnonymous { get; set; }

        [JsonProperty("comments")]
        public Comment[] Comments { get; set; }

    }
}
