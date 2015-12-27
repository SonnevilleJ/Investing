﻿using Ninject;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Sonneville.FidelityWebDriver.Demo.Ninject;

namespace Sonneville.FidelityWebDriver.Demo.Tests.Ninject
{
    [TestFixture]
    public class KernelBuilderTests
    {
        private IKernel _kernel;

        [SetUp]
        public void Setup()
        {
            _kernel = new KernelBuilder().Build();
        }

        [TearDown]
        public void TearDown()
        {
            _kernel?.Dispose();
        }

        [Test]
        public void ShouldGetApp()
        {
            using (var app = _kernel.Get<IApp>())
            {
                Assert.IsNotNull(app);
            }
        }

        [Test]
        public void ShouldGetChromeDriver()
        {
            using (var webDriver = _kernel.Get<IWebDriver>())
            {
                Assert.IsInstanceOf<ChromeDriver>(webDriver);
            }
        }
    }
}