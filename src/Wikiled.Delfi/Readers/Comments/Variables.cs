using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    public class Variables
    {
        [JsonProperty("articleId")]
        public long ArticleId { get; set; }

        [JsonProperty("channelId")]
        public long ChannelId { get; set; }

        [JsonProperty("modeType")]
        public string ModeType { get; set; }

        [JsonProperty("orderBy")]
        public string OrderBy { get; set; }

        [JsonProperty("limitReplies")]
        public long LimitReplies { get; set; }

        [JsonProperty("orderByReplies")]
        public string OrderByReplies { get; set; }
    }
}
