using System;
using Newtonsoft.Json;

namespace Wikiled.Delfi.Readers.Comments
{
    internal class CodeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Code) || t == typeof(Code?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "dk":
                    return Code.Dk;
                case "lt":
                    return Code.Lt;
            }
            throw new Exception("Cannot unmarshal type Code");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Code)untypedValue;
            switch (value)
            {
                case Code.Dk:
                    serializer.Serialize(writer, "dk");
                    return;
                case Code.Lt:
                    serializer.Serialize(writer, "lt");
                    return;
            }
            throw new Exception("Cannot marshal type Code");
        }

        public static readonly CodeConverter Singleton = new CodeConverter();
    }
}