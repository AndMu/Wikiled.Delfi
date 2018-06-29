using System.Threading.Tasks;
using Wikiled.Delfi.Data;

namespace Wikiled.Delfi.Readers
{
    public interface IArticleTextReader
    {
        Task<ArticleText> ReadArticle(ArticleDefinition definition);
    }
}