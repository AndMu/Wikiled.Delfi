using System;
using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    internal class ReactionTypenameConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ReactionTypename) || t == typeof(ReactionTypename?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "Reaction")
            {
                return ReactionTypename.Reaction;
            }
            throw new Exception("Cannot unmarshal type ReactionTypename");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ReactionTypename)untypedValue;
            if (value == ReactionTypename.Reaction)
            {
                serializer.Serialize(writer, "Reaction");
                return;
            }
            throw new Exception("Cannot marshal type ReactionTypename");
        }

        public static readonly ReactionTypenameConverter Singleton = new ReactionTypenameConverter();
    }
}