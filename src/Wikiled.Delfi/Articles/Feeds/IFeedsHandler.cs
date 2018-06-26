using System;

namespace Wikiled.Delfi.Articles.Feeds
{
    public interface IFeedsHandler
    {
        IObservable<ArticleDefinition> GetArticles();
    }
}