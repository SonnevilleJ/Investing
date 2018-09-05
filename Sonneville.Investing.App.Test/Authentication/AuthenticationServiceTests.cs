using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using Moq;
using NUnit.Framework;
using Sonneville.Investing.App.Authentication;
using Sonneville.Investing.Persistence.Users;
using Sonneville.Investing.Users;
using Sonneville.Utilities.Security;

namespace Sonneville.Investing.App.Test.Authentication
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private AuthenticationService _authenticationService;
        private Mock<IPasswordService> _mockPasswordService;
        private Mock<IUserRepository> _mockUserRepository;
        private string _password;
        private PasswordHash _passwordHash;

        [SetUp]
        public void Setup()
        {
            _password = "password";
            _passwordHash = new PasswordHash();

            var persistedUsers = new Dictionary<User, PasswordHash>();

            _mockPasswordService = new Mock<IPasswordService>();
            _mockPasswordService.Setup(service => service.HashPassword(_password))
                .Returns(_passwordHash);
            _mockPasswordService
                .Setup(service => service.ValidatePassword(It.IsAny<string>(), It.IsAny<PasswordHash>()))
                .Returns<string, PasswordHash>((password, passwordHash) =>
                    ValidatePasswordHash(_mockPasswordService.Object.HashPassword(password), passwordHash));

            _mockUserRepository = new Mock<IUserRepository>();
            _mockUserRepository
                .Setup(repository => repository.CreateNewUser(It.IsAny<User>(), It.IsAny<PasswordHash>()))
                .Callback<User, PasswordHash>((user, passwordHash) => persistedUsers.Add(user, passwordHash));
            _mockUserRepository
                .Setup(repository => repository.FindUserByUserName(It.IsAny<string>()))
                .Returns<string>(username => persistedUsers.Single(kvp => kvp.Key.UserName == username).Key);
            _mockUserRepository
                .Setup(repository => repository.FindPasswordByUserName(It.IsAny<string>()))
                .Returns<string>(username => persistedUsers.Single(kvp => kvp.Key.UserName == username).Value);

            _authenticationService = new AuthenticationService(_mockPasswordService.Object, _mockUserRepository.Object);
        }

        [Test]
        public void CreateNewUser()
        {
            var user = CreateUser();
            _authenticationService.CreateNewUser(user, _password);

            _mockUserRepository.Verify(userRepository => userRepository.CreateNewUser(
                It.Is<User>(entity => ValidateUser(entity, user)),
                It.Is<PasswordHash>(entity => ValidatePasswordHash(entity, _passwordHash))
            ));
        }

        [Test]
        public void ShouldLogInWithValidPassword()
        {
            var user = CreateUser();
            _authenticationService.CreateNewUser(user, _password);

            var authenticatedUser = _authenticationService.LogIn(user.UserName, _password);

            AssertEquals(user, authenticatedUser);
        }

        [Test]
        public void ShouldThrowOnLogInWithInvalidPassword()
        {
            var user = CreateUser();
            _authenticationService.CreateNewUser(user, _password);

            Assert.Throws<AuthenticationException>(
                () => _authenticationService.LogIn(user.UserName, _password + "fake"));
        }

        private static User CreateUser(int seed = 1)
        {
            return new User
            {
                FirstName = (seed * 1).ToString(),
                LastName = (seed * 2).ToString(),
                UserName = (seed * 3).ToString(),
            };
        }

        private static bool ValidateUser(User entity, User user)
        {
            if (entity == null) return false;
            AssertEquals(user, entity);
            return true;
        }

        private static bool ValidatePasswordHash(PasswordHash entity, PasswordHash passwordHash)
        {
            if (entity == null) return false;
            AssertEquals(passwordHash, entity);
            return true;
        }

        private static void AssertEquals(User user, User entity)
        {
            Assert.AreEqual(user.FirstName, entity.FirstName);
            Assert.AreEqual(user.LastName, entity.LastName);
            Assert.AreEqual(user.UserName, entity.UserName);
        }

        private static void AssertEquals(PasswordHash passwordHash, PasswordHash entity)
        {
            Assert.AreEqual(passwordHash.CryptorName, entity.CryptorName);
            Assert.AreEqual(passwordHash.HashAlgorithm, entity.HashAlgorithm);
            Assert.AreEqual(passwordHash.HashDigest, entity.HashDigest);
            Assert.AreEqual(passwordHash.Iterations, entity.Iterations);
            Assert.AreEqual(passwordHash.Salt, entity.Salt);
        }
    }
}