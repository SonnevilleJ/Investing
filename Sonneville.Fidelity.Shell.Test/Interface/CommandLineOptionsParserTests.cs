﻿using System;
using System.IO;
using Moq;
using NUnit.Framework;
using Sonneville.Fidelity.Shell.AppStartup;
using Sonneville.Fidelity.Shell.Configuration;
using Sonneville.Fidelity.Shell.Interface;
using Sonneville.FidelityWebDriver.Configuration;
using Sonneville.Investing.Trading;
using Sonneville.Utilities.Configuration;

namespace Sonneville.Fidelity.Shell.Test.AppStartup
{
    [TestFixture]
    public class CommandLineOptionsParserTests
    {
        private string _cliUserName;
        private string _cliPassword;
        private FidelityConfiguration _fidelityConfiguration;
        private CommandLineOptionsParser _optionsParser;
        private MemoryStream _memoryStream;
        private StreamWriter _streamWriter;
        private PortfolioManagerConfiguration _portfolioManagerConfiguration;
        private Mock<INiniConfigStore> _configStoreMock;

        [SetUp]
        public void Setup()
        {
            _cliUserName = "Batman";
            _cliPassword = "I am vengeance. I am the night. I am Batman.";

            _fidelityConfiguration = new FidelityConfiguration();

            _portfolioManagerConfiguration = new PortfolioManagerConfiguration();

            _configStoreMock = new Mock<INiniConfigStore>();
            _configStoreMock.Setup(configStore => configStore.Read<FidelityConfiguration>()).Returns(_fidelityConfiguration);
            _configStoreMock.Setup(configStore => configStore.Save(It.IsAny<FidelityConfiguration>())).Callback<FidelityConfiguration>(config => _fidelityConfiguration = config);
            _configStoreMock.Setup(configStore => configStore.Read<PortfolioManagerConfiguration>()).Returns(_portfolioManagerConfiguration);
            _configStoreMock.Setup(configStore => configStore.Save(It.IsAny<PortfolioManagerConfiguration>())).Callback<PortfolioManagerConfiguration>(config => _portfolioManagerConfiguration = config);
            
            _memoryStream = new MemoryStream();
            _streamWriter = new StreamWriter(_memoryStream) {AutoFlush = true};

            _optionsParser = new CommandLineOptionsParser(_configStoreMock.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _memoryStream.Dispose();
            _streamWriter.Dispose();
        }

        [Test]
        public void ShouldSetCredentialsFromCliArgsWithoutPersisting()
        {
            var args = new[] {"-u", _cliUserName, "-p", _cliPassword};

            var shouldExecute = _optionsParser.ShouldExecute(args, _streamWriter);

            Assert.IsTrue(shouldExecute);
            Assert.AreEqual(_cliUserName, _fidelityConfiguration.Username);
            Assert.AreEqual(_cliPassword, _fidelityConfiguration.Password);
            AssertUnchangedConfig();
        }

        [Test]
        public void ShouldSetCredentialsFromCliArgsAndPersist()
        {
            var args = new[] {"-u", _cliUserName, "-p", _cliPassword, "-s"};

            var shouldExecute = _optionsParser.ShouldExecute(args, _streamWriter);

            Assert.IsTrue(shouldExecute);
            var fidelityConfiguration = _fidelityConfiguration;
            Assert.AreEqual(_cliUserName, fidelityConfiguration.Username);
            Assert.AreEqual(_cliPassword, fidelityConfiguration.Password);
        }

        [Test]
        [TestCase("IA", AccountType.InvestmentAccount)]
        [TestCase("CC", AccountType.CreditCard)]
        [TestCase("HS", AccountType.HealthSavingsAccount)]
        [TestCase("OA", AccountType.Other)]
        [TestCase("RA", AccountType.RetirementAccount)]
        public void ShouldSetInScopeAccountTypesWithoutPersisting(string accountTypeCode, AccountType accountType)
        {
            var args = new[] {"-at", accountTypeCode};

            var shouldExecute = _optionsParser.ShouldExecute(args, _streamWriter);

            Assert.IsTrue(shouldExecute);
            CollectionAssert.Contains(_portfolioManagerConfiguration.InScopeAccountTypes, accountType);
            AssertUnchangedConfig();
        }

        [Test]
        [TestCase("IA", AccountType.InvestmentAccount)]
        [TestCase("CC", AccountType.CreditCard)]
        [TestCase("HS", AccountType.HealthSavingsAccount)]
        [TestCase("OA", AccountType.Other)]
        [TestCase("RA", AccountType.RetirementAccount)]
        public void ShouldSetInScopeAccountTypesAndPersist(string accountTypeCode, AccountType accountType)
        {
            var args = new[] {"-at", accountTypeCode, "-s"};

            var shouldExecute = _optionsParser.ShouldExecute(args, _streamWriter);

            Assert.IsTrue(shouldExecute);
            var configuration = _portfolioManagerConfiguration;
            CollectionAssert.Contains(configuration.InScopeAccountTypes, accountType);
        }

        [Test]
        [TestCase("XX")]
        public void ShouldIgnoreUnknownAccountTypes(string accountTypeCode)
        {
            var args = new[] {"-at", accountTypeCode};

            Assert.Throws<ArgumentException>(() => _optionsParser.ShouldExecute(args, _streamWriter));

            CollectionAssert.IsEmpty(_portfolioManagerConfiguration.InScopeAccountTypes);
        }

        [Test]
        public void ShouldDisplayHelpFromCliArgsAndNotPersist()
        {
            var args = new[] {"-u", _cliUserName, "-p", _cliPassword, "-s", "-h"};
            var shouldExecute = _optionsParser.ShouldExecute(args, _streamWriter);

            Assert.IsFalse(shouldExecute);
            var consoleOutput = ReadConsoleOutputFrom(_memoryStream);
            Assert.IsTrue(consoleOutput.Contains("-h"),
                $"Actual console output follows:{Environment.NewLine}{consoleOutput}");
            AssertUnchangedConfig();
        }

        private void AssertUnchangedConfig()
        {
            _configStoreMock.Verify(configStore => configStore.Save(It.IsAny<object>()), Times.Never);
        }

        private static string ReadConsoleOutputFrom(Stream memoryStream)
        {
            memoryStream.Position = 0;
            return new StreamReader(memoryStream).ReadToEnd();
        }
    }
}