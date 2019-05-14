using System;
using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    internal class CommentTypenameConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(CommentTypename) || t == typeof(CommentTypename?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "Comment")
            {
                return CommentTypename.Comment;
            }
            throw new Exception("Cannot unmarshal type CommentTypename");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (CommentTypename)untypedValue;
            if (value == CommentTypename.Comment)
            {
                serializer.Serialize(writer, "Comment");
                return;
            }
            throw new Exception("Cannot marshal type CommentTypename");
        }

        public static readonly CommentTypenameConverter Singleton = new CommentTypenameConverter();
    }
}