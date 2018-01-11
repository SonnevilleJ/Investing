using System.Collections.Generic;
using System.Linq;
using Ninject;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.Fidelity.Shell.AppStartup;
using Sonneville.Fidelity.Shell.Configuration;
using Sonneville.Fidelity.Shell.Interface;
using Sonneville.FidelityWebDriver;
using Sonneville.FidelityWebDriver.Configuration;
using Sonneville.FidelityWebDriver.Navigation;
using Sonneville.Utilities.Configuration;

namespace Sonneville.Fidelity.Shell.Test.AppStartup
{
    [TestFixture]
    public class ImmutableKernelTests
    {
        private static IKernel _kernel;

        [OneTimeSetUp]
        public void Setup()
        {
            _kernel = new KernelBuilder().Build();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _kernel?.Dispose();
        }

        [Test]
        public void ShouldBindConfigStoreAsSingleton()
        {
            var configStore = _kernel.Get<IConfigStore>();

            Assert.IsNotNull(configStore);
            Assert.AreSame(configStore, _kernel.Get<IConfigStore>());
        }

        [Test]
        public void ShouldCreateFidelityConfiguration()
        {
            var configStore = _kernel.Get<IConfigStore>();

            var config = configStore.Load<FidelityConfiguration>();
            Assert.IsNotNull(config);
        }

        [Test]
        public void ShouldCreateSeleniumConfiguration()
        {
            var configStore = _kernel.Get<IConfigStore>();

            var config = configStore.Load<SeleniumConfiguration>();
            Assert.IsNotNull(config);
        }

        [Test]
        public void ShouldBindApp()
        {
            using (var commandRouter = _kernel.Get<ICommandRouter>())
            {
                Assert.IsNotNull(commandRouter);
            }
        }

        [Test]
        public void ShouldBindCommands()
        {
            List<ICommand> commands = null;
            try
            {
                commands = _kernel.GetAll<ICommand>().ToList();

                Assert.IsNotEmpty(commands);
                CollectionAssert.AllItemsAreNotNull(commands);
            }
            finally
            {
                commands?.ForEach(command => command.Dispose());
            }
        }

        [Test]
        public void ShouldBindWebDriverAsSingleton()
        {
            using (var webDriver = _kernel.Get<IWebDriver>())
            {
                Assert.IsNotNull(webDriver);
                Assert.AreSame(webDriver, _kernel.Get<IWebDriver>());
            }
        }

        [Test]
        public void ShouldGetAllPages()
        {
            var pages = _kernel.GetAll<IPage>().ToList();

            Assert.IsNotEmpty(pages);
            CollectionAssert.AllItemsAreNotNull(pages);
        }

        [Test]
        public void ShouldGetSiteNavigator()
        {
            using (var siteNavigator = _kernel.Get<ISiteNavigator>())
            {
                Assert.IsNotNull(siteNavigator);
            }
        }
    }
}
