using System;
using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    internal class ReactionEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ReactionEnum) || t == typeof(ReactionEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "angry":
                    return ReactionEnum.Angry;
                case "dislike":
                    return ReactionEnum.Dislike;
                case "haha":
                    return ReactionEnum.Haha;
                case "like":
                    return ReactionEnum.Like;
                case "love":
                    return ReactionEnum.Love;
                case "sad":
                    return ReactionEnum.Sad;
                case "surprised":
                    return ReactionEnum.Surprised;
            }
            throw new Exception("Cannot unmarshal type ReactionEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ReactionEnum)untypedValue;
            switch (value)
            {
                case ReactionEnum.Angry:
                    serializer.Serialize(writer, "angry");
                    return;
                case ReactionEnum.Dislike:
                    serializer.Serialize(writer, "dislike");
                    return;
                case ReactionEnum.Haha:
                    serializer.Serialize(writer, "haha");
                    return;
                case ReactionEnum.Like:
                    serializer.Serialize(writer, "like");
                    return;
                case ReactionEnum.Love:
                    serializer.Serialize(writer, "love");
                    return;
                case ReactionEnum.Sad:
                    serializer.Serialize(writer, "sad");
                    return;
                case ReactionEnum.Surprised:
                    serializer.Serialize(writer, "surprised");
                    return;
            }
            throw new Exception("Cannot marshal type ReactionEnum");
        }

        public static readonly ReactionEnumConverter Singleton = new ReactionEnumConverter();
    }
}