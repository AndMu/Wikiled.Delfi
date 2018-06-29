using System;
using Wikiled.Delfi.Data;

namespace Wikiled.Delfi.Feeds
{
    public interface IFeedsHandler
    {
        IObservable<ArticleDefinition> GetArticles();
    }
}