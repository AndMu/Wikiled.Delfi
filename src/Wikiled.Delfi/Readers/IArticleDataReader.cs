using System.Threading.Tasks;
using Wikiled.Delfi.Data;

namespace Wikiled.Delfi.Readers
{
    public interface IArticleDataReader
    {
        Task<bool> RequiresRefreshing(Article article);

        Task<Article> Read(ArticleDefinition definition);
    }
}