using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Login;
using Sonneville.FidelityWebDriver.Positions;

namespace Sonneville.FidelityWebDriver.Tests.Positions
{
    [TestFixture]
    public class PositionsManagerTests : ManagerTestsBase<IPositionsManager>
    {
        private Mock<ILoginManager> _loginManagerMock;
        private Mock<IPositionsPage> _positionsPage;
        private List<IAccountSummary> _accountSummaries;
        private List<IAccountDetails> _accountDetails;

        protected override IPositionsManager InstantiateManager()
        {
            _accountSummaries = new List<IAccountSummary>();
            _accountDetails = new List<IAccountDetails>();

            _positionsPage = new Mock<IPositionsPage>();
            _positionsPage.Setup(positionsPage => positionsPage.GetAccountSummaries())
                .Returns(_accountSummaries);
            _positionsPage.Setup(positionsPage => positionsPage.GetAccountDetails())
                .Returns(_accountDetails);

            SiteNavigatorMock.Setup(siteNavigator => siteNavigator.GoTo<IPositionsPage>()).Returns(_positionsPage.Object);

            _loginManagerMock = new Mock<ILoginManager>();

            return new PositionsManager(LogMock.Object, SiteNavigatorMock.Object, _loginManagerMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            SetupTestsBase();
        }

        [Test]
        public void ShouldDisposeLoginManager()
        {
            Manager.Dispose();

            _loginManagerMock.Verify(loginManager => loginManager.Dispose());
        }

        [Test]
        public void ShouldReturnAccountSummariesFromPositionsPage()
        {
            var accounts = Manager.GetAccountSummaries();

            Assert.AreSame(_accountSummaries, accounts);
        }

        [Test]
        public void ShouldReturnAccountDetailsFromPositionsPage()
        {
            var accounts = Manager.GetAccountDetails();

            Assert.AreSame(_accountDetails, accounts);
        }

        [Test]
        public void ShouldEnsureLoggedInBeforeGettingAccountSummaries()
        {
            _positionsPage.Setup(page => page.GetAccountSummaries())
                .Callback(() => _loginManagerMock.Verify(loginManager => loginManager.EnsureLoggedIn()));

            Manager.GetAccountSummaries();

            _positionsPage.Verify(page => page.GetAccountSummaries());
        }

        [Test]
        public void ShouldEnsureLoggedInBeforeGettingAccountDetails()
        {
            _positionsPage.Setup(page => page.GetAccountDetails())
                .Callback(() => _loginManagerMock.Verify(loginManager => loginManager.EnsureLoggedIn()));

            Manager.GetAccountDetails();

            _positionsPage.Verify(page => page.GetAccountDetails());
        }
    }
}
