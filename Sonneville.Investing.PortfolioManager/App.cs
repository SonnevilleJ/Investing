using System;
using System.Collections.Generic;

namespace Sonneville.Investing.PortfolioManager
{
    public interface IApp : IDisposable
    {
        void Run(IEnumerable<string> args);
    }

    public class App : IApp
    {
        public void Run(IEnumerable<string> args)
        {
        }

        public void Dispose()
        {
        }
    }
}