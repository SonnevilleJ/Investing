using System;

namespace Sonneville.AssessorsAdapter.Scraper.Bootstrap
{
    public interface IApp : IDisposable
    {
        void Run(string[] args);
    }
}
