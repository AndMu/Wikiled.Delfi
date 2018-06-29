using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Wikiled.Delfi.Readers
{
    public interface IHtmlReader
    {
        Task<HtmlDocument> ReadDocument(string url);
    }
}