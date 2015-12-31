using System;
using System.IO;
using NUnit.Framework;
using Sonneville.FidelityWebDriver.Configuration;
using Sonneville.Investing.PortfolioManager.AppStartup;

namespace Sonneville.Investing.PortfolioManager.Test.AppStartup
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

        [SetUp]
        public void Setup()
        {
            _cliUserName = "Batman";
            _cliPassword = "I am vengeance. I am the night. I am Batman.";

            _fidelityConfiguration = new FidelityConfiguration();
            _fidelityConfiguration.Initialize();

            _memoryStream = new MemoryStream();
            _streamWriter = new StreamWriter(_memoryStream) {AutoFlush = true};

            _optionsParser = new CommandLineOptionsParser();
        }

        [TearDown]
        public void Teardown()
        {
            _memoryStream.Dispose();
            _streamWriter.Dispose();

            _fidelityConfiguration.Username = null;
            _fidelityConfiguration.Password = null;
            _fidelityConfiguration.Write();
        }

        [Test]
        public void ShouldSetConfigFromCliArgsWithoutPersisting()
        {
            var args = new[] {"-u", _cliUserName, "-p", _cliPassword};

            var shouldExecute = _optionsParser.ShouldExecute(args, _fidelityConfiguration, _streamWriter);

            Assert.IsTrue(shouldExecute);
            Assert.AreEqual(_cliUserName, _fidelityConfiguration.Username);
            Assert.AreEqual(_cliPassword, _fidelityConfiguration.Password);
            AssertUnchangedConfig();
        }

        [Test]
        public void ShouldSetConfigFromCliArgsAndPersist()
        {
            var args = new[] {"-u", _cliUserName, "-p", _cliPassword, "-s"};

            var shouldExecute = _optionsParser.ShouldExecute(args, _fidelityConfiguration, _streamWriter);

            Assert.IsTrue(shouldExecute);
            var fidelityConfiguration = new FidelityConfiguration();
            fidelityConfiguration.Initialize();
            Assert.AreEqual(_cliUserName, fidelityConfiguration.Username);
            Assert.AreEqual(_cliPassword, fidelityConfiguration.Password);
        }

        [Test]
        public void ShouldDisplayHelpFromCliArgsAndNotPersist()
        {
            var args = new[] {"-u", _cliUserName, "-p", _cliPassword, "-s", "-h"};
            var shouldExecute = _optionsParser.ShouldExecute(args, _fidelityConfiguration, _streamWriter);

            Assert.IsFalse(shouldExecute);
            var consoleOutput = ReadConsoleOutputFrom(_memoryStream);
            Assert.IsTrue(consoleOutput.Contains("-h"),
                $"Actual console output follows:{Environment.NewLine}{consoleOutput}");
            AssertUnchangedConfig();
        }

        private void AssertUnchangedConfig()
        {
            _fidelityConfiguration.Read();
            Assert.IsNull(_fidelityConfiguration.Username);
            Assert.IsNull(_fidelityConfiguration.Password);
        }

        private static string ReadConsoleOutputFrom(Stream memoryStream)
        {
            memoryStream.Position = 0;
            return new StreamReader(memoryStream).ReadToEnd();
        }
    }
}