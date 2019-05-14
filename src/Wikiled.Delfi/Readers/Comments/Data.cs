using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    public class Data
    {
        [JsonProperty("getCommentsByConfig")]
        public GetCommentsByConfig GetCommentsByConfig { get; set; }
    }
}