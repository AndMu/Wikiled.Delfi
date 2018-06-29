using System;
using System.Threading.Tasks;
using Wikiled.Delfi.Data;

namespace Wikiled.Delfi.Readers
{
    public interface ICommentsReader
    {
        Task Init();

        IObservable<CommentData> ReadAllComments();
    }
}