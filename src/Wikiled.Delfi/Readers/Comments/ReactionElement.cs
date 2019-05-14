using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    public class ReactionElement
    {
        [JsonProperty("comment_id")]
        public long CommentId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("reaction")]
        public string Reaction { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }
    }
}