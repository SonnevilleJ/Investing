using NUnit.Framework;

namespace Sonneville.Investing.PortfolioManager.Test
{
    [TestFixture]
    public class AppTests
    {
        private App _app;

        [SetUp]
        public void Setup()
        {
            _app = new App();
        }

        [Test]
        public void DisposeShouldNotThrow()
        {
            _app.Dispose();
            _app.Dispose();
        }

        [Test]
        public void RunShouldNotThrow()
        {
            _app.Run(null);
        }
    }
}