using System;
using NUnit.Framework;
using Sonneville.AssessorsAdapter.Scraper.Bootstrap;

namespace Sonneville.AssessorsAdapter.Scraper.Test.Bootstrap
{
    [TestFixture]
    public class AppTests
    {
        [SetUp]
        public void Setup()
        {
            _app = new App();
        }

        private App _app;

        [Test]
        public void ShouldRun()
        {
            throw new NotImplementedException();
        }
    }
}
