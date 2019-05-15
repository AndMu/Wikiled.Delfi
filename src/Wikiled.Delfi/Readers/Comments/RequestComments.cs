using System;
using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    public class RequestComments
    {
        [JsonProperty("operationName")]
        public string OperationName { get; set; }

        [JsonProperty("variables")]
        public Variables Variables { get; set; }

        [JsonProperty("query")]
        public string Query { get; set; }

        public static RequestComments Create(long articleId, bool registered = true, int pageSize = 20, int page = 0)
        {
            if (pageSize > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Maximum page should be 100");
            }

            var request = new RequestComments();
            request.OperationName = "cfe_getCommentsByConfig";
            request.Variables = new Variables();
            request.Variables.ArticleId = articleId;
            request.Variables.ChannelId = 500;
            request.Variables.LimitReplies = 10;
            request.Variables.Limit = pageSize;
            if (page > 0)
            {
                request.Variables.Offset = page * pageSize;
            }

            request.Variables.ModeType = registered ? "REGISTERED" : "ANONYMOUS";
            request.Variables.OrderBy = "DATE_ASC";
            request.Variables.OrderByReplies = "DATE_ASC";
            request.Query =
                "fragment Commentbody on Comment {\n  id\n  subject\n  content\n  created_time\n  created_time_unix\n  client_ip\n  client_cookie\n  vote {\n    up\n    down\n    sum\n    __typename\n  }\n  author {\n    id\n    customer_id\n    idp_id\n    display_name\n    __typename\n  }\n  parent_comment {\n    id\n    subject\n    __typename\n  }\n  quote_to_comment {\n    id\n    subject\n    __typename\n  }\n  reaction {\n    comment_id\n    name\n    reaction\n    count\n    __typename\n  }\n  country_code {\n    name\n    code\n    __typename\n  }\n  count_replies\n  count_registered_replies\n  status\n  __typename\n}\n\nquery cfe_getCommentsByConfig($articleId: Int!, $channelId: Int!, $offset: Int! $modeType: ModeType!, $orderBy: OrderBy, $limitReplies: Int, $limit: Int, $orderByReplies: OrderBy) {\n  getCommentsByConfig(articleId: $articleId, channelId: $channelId) {\n    config {\n      enabled\n      comments_per_page\n      premoderated\n      premoderate_registered_users\n      registered_thread_enabled\n      user_can_delete_comments\n      disable_votes\n      registered_comments_only\n      __typename\n    }\n    articleEntity {\n      article_id\n      count_total\n      count_total_main_posts\n      count_registered\n      count_registered_main_posts\n      count_anonymous_main_posts\n      count_anonymous\n      comments(mode_type: $modeType, offset: $offset, limit: $limit, orderBy: $orderBy) {\n        ...Commentbody\n        replies(limit: $limitReplies, orderBy: $orderByReplies) {\n          ...Commentbody\n          __typename\n        }\n        __typename\n      }\n      __typename\n    }\n    __typename\n  }\n}\n";
            return request;
        }
    }
}
