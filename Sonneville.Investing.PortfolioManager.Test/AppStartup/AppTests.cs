using System;
using Moq;
using NUnit.Framework;
using Sonneville.FidelityWebDriver.Configuration;
using Sonneville.Investing.PortfolioManager.AppStartup;

namespace Sonneville.Investing.PortfolioManager.Test.AppStartup
{
    [TestFixture]
    public class AppTests
    {
        private App _app;
        private Mock<IAccountRebalancer> _accountRebalancerMock;
        private FidelityConfiguration _fidelityConfiguration;
        private Mock<ICommandLineOptionsParser> _optionsParserMock;
        private string[] _cliArgs;

        [SetUp]
        public void Setup()
        {
            _cliArgs = new string[0];

            _fidelityConfiguration = new FidelityConfiguration();
            _fidelityConfiguration.Initialize();

            _optionsParserMock = new Mock<ICommandLineOptionsParser>();
            _optionsParserMock.Setup(parser => parser.ShouldExecute(_cliArgs, _fidelityConfiguration, Console.Out)).Returns(true);

            _accountRebalancerMock = new Mock<IAccountRebalancer>();

            _app = new App(_fidelityConfiguration, _optionsParserMock.Object, _accountRebalancerMock.Object);
        }

        [Test]
        public void DisposeShouldNotThrow()
        {
            _app.Dispose();
            _app.Dispose();

            _accountRebalancerMock.Verify(rebalancer => rebalancer.Dispose());
        }

        [Test]
        public void ShouldExitWhenOptionsParserReturnsFalse()
        {
            _optionsParserMock = new Mock<ICommandLineOptionsParser>();
            _optionsParserMock.Setup(parser => parser.ShouldExecute(_cliArgs, _fidelityConfiguration, Console.Out)).Returns(false);
            _app = new App(_fidelityConfiguration, _optionsParserMock.Object, new Mock<IAccountRebalancer>(MockBehavior.Strict).Object);

            _app.Run(_cliArgs);
        }

        [Test]
        public void ShouldRebalanceAccounts()
        {
            _app.Run(_cliArgs);

            _accountRebalancerMock.Verify(rebalancer => rebalancer.RebalanceAccounts());
        }
    }
}