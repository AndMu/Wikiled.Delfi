using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    public class ParentCommentClass
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("__typename")]
        public CommentTypename Typename { get; set; }
    }
}