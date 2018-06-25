using System;
using System.Threading.Tasks;

namespace Wikiled.Delfi.Articles
{
    public interface ICommentsReader
    {
        Task Init();

        IObservable<CommentData> ReadAllComments();
    }
}