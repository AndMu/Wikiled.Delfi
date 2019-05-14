using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    public class Config
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("comments_per_page")]
        public long CommentsPerPage { get; set; }

        [JsonProperty("premoderated")]
        public bool Premoderated { get; set; }

        [JsonProperty("premoderate_registered_users")]
        public bool PremoderateRegisteredUsers { get; set; }

        [JsonProperty("registered_thread_enabled")]
        public bool RegisteredThreadEnabled { get; set; }

        [JsonProperty("user_can_delete_comments")]
        public bool UserCanDeleteComments { get; set; }

        [JsonProperty("disable_votes")]
        public bool DisableVotes { get; set; }

        [JsonProperty("registered_comments_only")]
        public bool RegisteredCommentsOnly { get; set; }
    }
}