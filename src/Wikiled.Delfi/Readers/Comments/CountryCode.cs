using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    public class CountryCode
    {
        [JsonProperty("name")]
        public object Name { get; set; }

        [JsonProperty("code")]
        public Code Code { get; set; }

        [JsonProperty("__typename")]
        public CountryCodeTypename Typename { get; set; }
    }
}