using System;
using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    internal class NameConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Name) || t == typeof(Name?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Angry":
                    return Name.Angry;
                case "Dislike":
                    return Name.Dislike;
                case "Ha-Ha":
                    return Name.HaHa;
                case "Like":
                    return Name.Like;
                case "Love":
                    return Name.Love;
                case "Sad":
                    return Name.Sad;
                case "Surprised":
                    return Name.Surprised;
            }
            throw new Exception("Cannot unmarshal type Name");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Name)untypedValue;
            switch (value)
            {
                case Name.Angry:
                    serializer.Serialize(writer, "Angry");
                    return;
                case Name.Dislike:
                    serializer.Serialize(writer, "Dislike");
                    return;
                case Name.HaHa:
                    serializer.Serialize(writer, "Ha-Ha");
                    return;
                case Name.Like:
                    serializer.Serialize(writer, "Like");
                    return;
                case Name.Love:
                    serializer.Serialize(writer, "Love");
                    return;
                case Name.Sad:
                    serializer.Serialize(writer, "Sad");
                    return;
                case Name.Surprised:
                    serializer.Serialize(writer, "Surprised");
                    return;
            }
            throw new Exception("Cannot marshal type Name");
        }

        public static readonly NameConverter Singleton = new NameConverter();
    }
}