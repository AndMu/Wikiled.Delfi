using System;
using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    public class Comment
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("created_time")]
        public DateTimeOffset CreatedTime { get; set; }

        [JsonProperty("created_time_unix")]
        public long CreatedTimeUnix { get; set; }

        [JsonProperty("client_ip")]
        public object ClientIp { get; set; }

        [JsonProperty("client_cookie")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ClientCookie { get; set; }

        [JsonProperty("vote")]
        public Vote Vote { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; }

        [JsonProperty("parent_comment")]
        public ParentCommentClass ParentComment { get; set; }

        [JsonProperty("quote_to_comment")]
        public ParentCommentClass QuoteToComment { get; set; }

        [JsonProperty("reaction")]
        public ReactionElement[] Reaction { get; set; }

        [JsonProperty("country_code")]
        public CountryCode CountryCode { get; set; }

        [JsonProperty("count_replies")]
        public long CountReplies { get; set; }

        [JsonProperty("count_registered_replies")]
        public long CountRegisteredReplies { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }

        [JsonProperty("__typename")]
        public CommentTypename Typename { get; set; }

        [JsonProperty("replies")]
        public Comment[] Replies { get; set; }
    }
}