using System;
using Moq;
using NUnit.Framework;
using Sonneville.Investing.Persistence.EFCore.EntityFrameworkCore;
using Sonneville.Investing.Persistence.EFCore.Users;
using Sonneville.Investing.Persistence.Users;
using Sonneville.Investing.Users;

namespace Sonneville.Investing.Persistence.Test.Users
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private IUserRepository _userRepository;
        private Mock<IDataContext> _mockDataContext;
        private Mock<IApplicationUserRepository> _mockApplicationUserRepository;

        [SetUp]
        public void Setup()
        {
            _mockApplicationUserRepository = new Mock<IApplicationUserRepository>();

            _mockDataContext = new Mock<IDataContext>();
            _mockDataContext.Setup(dataContext => dataContext.ApplicationUserRepository)
                .Returns(_mockApplicationUserRepository.Object);

            _userRepository = new PersistenceContext(_mockDataContext.Object).UserRepository;
        }

        [Test]
        public void ShouldMapReturnedUsers()
        {
            var applicationUser = CreateApplicationUser("Bruce", "Wayne", "Batman");
            _mockApplicationUserRepository.Setup(repository => repository.FindByUserName(applicationUser.UserName))
                .Returns(applicationUser);

            var actual = _userRepository.FindByUserName(applicationUser.UserName);

            AssertEquals(applicationUser, actual);
        }

        [Test]
        public void ShouldReturnNullForMissingUsers()
        {
            const string userName = "missing";

            var found = _userRepository.FindByUserName(userName);

            Assert.IsNull(found);
        }

        [Test]
        public void ShouldCreateNewUser()
        {
            var user = CreateUser("Bruce", "Wayne", "Batman");

            _userRepository.CreateNewUser(user);

            _mockApplicationUserRepository.Verify(repository =>
                repository.Add(It.Is<ApplicationUser>(entity => AssertEquals(entity, user)))
            );
        }

        [Test]
        public void ShouldThrowIfCreatingDuplicateUserName()
        {
            var lateUser = CreateUser("Clark", "Kent", "Batman");
            _mockApplicationUserRepository.Setup(repository => repository.IsUserNameTaken(lateUser.UserName))
                .Returns(true);

            Assert.Throws<InvalidOperationException>(() => _userRepository.CreateNewUser(lateUser));
        }

        [Test]
        public void ShouldReturnUserNameAvailable()
        {
            var userNameIsAvailable = _userRepository.UserNameIsAvailable("Batman");

            Assert.IsTrue(userNameIsAvailable);
        }

        [Test]
        public void ShouldReturnUserNameUnavailable()
        {
            const string userName = "username";
            _mockApplicationUserRepository.Setup(repository => repository.IsUserNameTaken(userName)).Returns(true);

            var userNameIsAvailable = _userRepository.UserNameIsAvailable(userName);

            Assert.IsFalse(userNameIsAvailable);
        }

        [Test]
        public void ShouldDisposeApplicationUserRepository()
        {
            _userRepository.Dispose();

            _mockApplicationUserRepository.Verify(repository => repository.Dispose());
        }

        [Test]
        public void ShouldDisposeDataContext()
        {
            _userRepository.Dispose();

            _mockDataContext.Verify(context => context.Dispose());
        }

        private static ApplicationUser CreateApplicationUser(string firstName, string lastName, string userName)
        {
            return new ApplicationUser
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
            };
        }

        private static User CreateUser(string firstName, string lastName, string userName)
        {
            return new User
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
            };
        }

        private bool AssertEquals(ApplicationUser entity, User user)
        {
            Assert.AreEqual(entity.FirstName, user.FirstName);
            Assert.AreEqual(entity.LastName, user.LastName);
            Assert.AreEqual(entity.UserName, user.UserName);
            return true;
        }
    }
}
