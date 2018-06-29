using System.Threading.Tasks;
using Wikiled.Delfi.Data;

namespace Wikiled.Delfi.Readers
{
    public interface IArticleDataReader
    {
        Task<Article> Read(ArticleDefinition definition);
    }
}