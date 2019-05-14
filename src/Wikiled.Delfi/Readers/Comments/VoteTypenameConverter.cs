using System;
using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    internal class VoteTypenameConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(VoteTypename) || t == typeof(VoteTypename?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "Vote")
            {
                return VoteTypename.Vote;
            }
            throw new Exception("Cannot unmarshal type VoteTypename");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (VoteTypename)untypedValue;
            if (value == VoteTypename.Vote)
            {
                serializer.Serialize(writer, "Vote");
                return;
            }
            throw new Exception("Cannot marshal type VoteTypename");
        }

        public static readonly VoteTypenameConverter Singleton = new VoteTypenameConverter();
    }
}