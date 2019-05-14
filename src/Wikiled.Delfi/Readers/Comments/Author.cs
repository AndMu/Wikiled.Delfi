using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    public class Author
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("customer_id")]
        public long CustomerId { get; set; }

        [JsonProperty("idp_id")]
        public object IdpId { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

    }
}