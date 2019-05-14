using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Wikiled.Delfi.Readers.Comments
{
    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                CommentTypenameConverter.Singleton,
                AuthorTypenameConverter.Singleton,
                CountryCodeTypenameConverter.Singleton,
                CodeConverter.Singleton,
                ReactionTypenameConverter.Singleton,
                NameConverter.Singleton,
                ReactionEnumConverter.Singleton,
                StatusConverter.Singleton,
                VoteTypenameConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}