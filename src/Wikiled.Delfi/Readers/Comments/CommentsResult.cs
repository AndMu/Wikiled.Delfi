using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    public class CommentsResult
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }
}