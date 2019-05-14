using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    public class Vote
    {
        [JsonProperty("up")]
        public long Up { get; set; }

        [JsonProperty("down")]
        public long Down { get; set; }

        [JsonProperty("sum")]
        public long Sum { get; set; }

        [JsonProperty("__typename")]
        public VoteTypename Typename { get; set; }
    }
}