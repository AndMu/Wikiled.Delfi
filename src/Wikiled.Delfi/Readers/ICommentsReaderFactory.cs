using Wikiled.Delfi.Data;

namespace Wikiled.Delfi.Readers
{
    public interface ICommentsReaderFactory
    {
        ICommentsReader Create(ArticleDefinition article, bool anonymous);
    }
}