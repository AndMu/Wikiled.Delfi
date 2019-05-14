using System;
using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    internal class AuthorTypenameConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(AuthorTypename) || t == typeof(AuthorTypename?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "User")
            {
                return AuthorTypename.User;
            }
            throw new Exception("Cannot unmarshal type AuthorTypename");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (AuthorTypename)untypedValue;
            if (value == AuthorTypename.User)
            {
                serializer.Serialize(writer, "User");
                return;
            }
            throw new Exception("Cannot marshal type AuthorTypename");
        }

        public static readonly AuthorTypenameConverter Singleton = new AuthorTypenameConverter();
    }
}