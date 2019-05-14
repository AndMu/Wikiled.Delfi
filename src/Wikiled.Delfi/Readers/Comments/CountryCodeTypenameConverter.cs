using System;
using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    internal class CountryCodeTypenameConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(CountryCodeTypename) || t == typeof(CountryCodeTypename?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "CountryCode")
            {
                return CountryCodeTypename.CountryCode;
            }
            throw new Exception("Cannot unmarshal type CountryCodeTypename");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (CountryCodeTypename)untypedValue;
            if (value == CountryCodeTypename.CountryCode)
            {
                serializer.Serialize(writer, "CountryCode");
                return;
            }
            throw new Exception("Cannot marshal type CountryCodeTypename");
        }

        public static readonly CountryCodeTypenameConverter Singleton = new CountryCodeTypenameConverter();
    }
}