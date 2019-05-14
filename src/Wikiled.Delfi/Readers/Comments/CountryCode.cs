using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    public class CountryCode
    {
        [JsonProperty("name")]
        public object Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }
}