using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    public class ReactionElement
    {
        [JsonProperty("comment_id")]
        public long CommentId { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }

        [JsonProperty("reaction")]
        public ReactionEnum Reaction { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("__typename")]
        public ReactionTypename Typename { get; set; }
    }
}