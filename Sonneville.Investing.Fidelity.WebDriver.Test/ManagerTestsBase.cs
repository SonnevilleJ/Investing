using log4net;
using Moq;
using NUnit.Framework;
using Sonneville.Investing.Fidelity.WebDriver.Configuration;
using Sonneville.Investing.Fidelity.WebDriver.Navigation;

namespace Sonneville.Investing.Fidelity.WebDriver.Test
{
    [TestFixture]
    public abstract class ManagerTestsBase<T> where T : IManager
    {
        protected Mock<ILog> LogMock;
        protected T Manager;
        protected Mock<ISiteNavigator> SiteNavigatorMock;
        protected FidelityConfiguration FidelityConfiguration;

        [SetUp]
        public void SetupTestsBase()
        {
            LogMock = new Mock<ILog>();
            SiteNavigatorMock = new Mock<ISiteNavigator>();
            FidelityConfiguration = new FidelityConfiguration();

            Manager = InstantiateManager();
        }

        protected abstract T InstantiateManager();

        [Test]
        public void EachManagerShouldDisposeSiteNavigator()
        {
            Manager.Dispose();

            SiteNavigatorMock.Verify(driver => driver.Dispose());
        }

        [Test]
        public void EachManagerShouldHandleMultipleDisposals()
        {
            Manager.Dispose();
            Manager.Dispose();
        }
    }
}