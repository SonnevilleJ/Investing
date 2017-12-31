using log4net;
using Moq;
using NUnit.Framework;
using Sonneville.FidelityWebDriver.Configuration;
using Sonneville.FidelityWebDriver.Navigation;

namespace Sonneville.FidelityWebDriver.Tests
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