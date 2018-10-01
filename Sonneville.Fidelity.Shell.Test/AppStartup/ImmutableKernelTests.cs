using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Repository.Hierarchy;
using Ninject;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.Fidelity.Shell.AppStartup.Ninject;
using Sonneville.Fidelity.Shell.Interface;
using Sonneville.Investing.Fidelity.WebDriver;
using Sonneville.Investing.Fidelity.WebDriver.Configuration;
using Sonneville.Investing.Fidelity.WebDriver.Navigation;
using Sonneville.Investing.Persistence;
using Sonneville.Investing.Persistence.EFCore.EntityFrameworkCore;
using Sonneville.Utilities.Persistence.v2;

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

            LogManager.Shutdown();
        }

        [Test]
        public void ShouldGetDatabaseConnectionInfo()
        {
            Assert.Throws<NotSupportedException>(() => _kernel.Get<DatabaseConnectionInfo>());
        }

        [Test]
        public void ShouldGetPersistenceContext()
        {
            Assert.Throws<NotSupportedException>(() => _kernel.Get<IPersistenceContext>());
        }

        [Test]
        public void ShouldBindConfigStoreAsSingleton()
        {
            var dataStore = _kernel.Get<IDataStore>();

            Assert.IsNotNull(dataStore);
            Assert.AreSame(dataStore, _kernel.Get<IDataStore>());
        }

        [Test]
        public void ShouldCreateFidelityConfiguration()
        {
            var dataStore = _kernel.Get<IDataStore>();

            var config = dataStore.Load<FidelityConfiguration>();
            Assert.IsNotNull(config);
        }

        [Test]
        public void ShouldCreateSeleniumConfiguration()
        {
            var dataStore = _kernel.Get<IDataStore>();

            var config = dataStore.Load<SeleniumConfiguration>();
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
            AssertSingleton<IWebDriver>();
        }

        private static void AssertSingleton<T>() where T : IDisposable
        {
            using (var first = _kernel.Get<T>())
            {
                Assert.IsNotNull(first);
                using (var second = _kernel.Get<T>())
                {
                    Assert.AreSame(first, second);
                }
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
