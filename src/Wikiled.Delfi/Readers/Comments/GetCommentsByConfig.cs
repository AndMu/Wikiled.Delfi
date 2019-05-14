using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    public class GetCommentsByConfig
    {
        [JsonProperty("config")]
        public Config Config { get; set; }

        [JsonProperty("articleEntity")]
        public ArticleEntity ArticleEntity { get; set; }
    }
}