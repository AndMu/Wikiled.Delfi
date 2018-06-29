using System;

namespace Wikiled.Delfi.Data
{
    public class Article
    {
        public Article(ArticleDefinition definition, CommentData[] comments, ArticleText articleText)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            Comments = comments ?? throw new ArgumentNullException(nameof(comments));
            ArticleText = articleText ?? throw new ArgumentNullException(nameof(articleText));
        }

        public ArticleDefinition Definition { get; }

        public CommentData[] Comments{ get; }

        public ArticleText ArticleText { get; }
    }
}
