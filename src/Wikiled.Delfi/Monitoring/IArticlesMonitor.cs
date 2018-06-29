using System;
using Wikiled.Delfi.Data;

namespace Wikiled.Delfi.Monitoring
{
    public interface IArticlesMonitor
    {
        IObservable<Article> Start();
    }
}