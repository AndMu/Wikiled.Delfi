using System;
using System.Linq;
using System.Reactive.Linq;
using Wikiled.Delfi.Adjusters;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers;

namespace Wikiled.Delfi.Readers
{
    public class CommentsReader : ICommentsReader
    {
        private readonly ISpecificCommentsReader[] readers;

        public CommentsReader(ArticleDefinition article, IAdjuster[] adjusters, Func<ArticleDefinition, IAdjuster, ISpecificCommentsReader> readerFactory)
        {
            if (adjusters == null)
            {
                throw new ArgumentNullException(nameof(adjusters));
            }

            if (readerFactory == null)
            {
                throw new ArgumentNullException(nameof(readerFactory));
            }

            readers = adjusters.Select(item => readerFactory(article, item)).ToArray();
        }

        public IObservable<CommentData> ReadAllComments()
        {
            return readers.Select(item => item.ReadAllComments()).Concat();
        }
    }
}
