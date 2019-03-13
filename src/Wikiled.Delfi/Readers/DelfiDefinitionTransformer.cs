using System;
using System.Text.RegularExpressions;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers;

namespace Wikiled.Delfi.Readers
{
    public class DelfiDefinitionTransformer : IDefinitionTransformer
    {
        public ArticleDefinition Transform(ArticleDefinition definition)
        {
            definition.Date = DateTime.UtcNow;
            definition.Id = Regex.Replace(definition.Id, @".*\?id=(.+)", "$1");
            return definition;
        }
    }
}
