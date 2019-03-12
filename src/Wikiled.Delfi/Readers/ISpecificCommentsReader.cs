using Wikiled.News.Monitoring.Readers;

namespace Wikiled.Delfi.Readers
{
    public interface ISpecificCommentsReader : ICommentsReader
    {
        int Total { get; }
    }
}