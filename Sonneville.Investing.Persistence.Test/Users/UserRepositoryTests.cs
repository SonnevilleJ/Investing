using System;
using Moq;
using NUnit.Framework;
using Sonneville.Investing.Persistence.EFCore.EntityFrameworkCore;
using Sonneville.Investing.Persistence.EFCore.Security;
using Sonneville.Investing.Persistence.EFCore.Users;
using Sonneville.Investing.Persistence.Users;
using Sonneville.Investing.Users;
using Sonneville.Utilities.Security;

namespace Sonneville.Investing.Persistence.Test.Users
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private IUserRepository _userRepository;
        private Mock<IDataContext> _mockDataContext;
        private Mock<IApplicationUserRepository> _mockApplicationUserRepository;
        private Mock<IPasswordDigestRepository> _mockPasswordDigestRepository;

        [SetUp]
        public void Setup()
        {
            _mockApplicationUserRepository = new Mock<IApplicationUserRepository>();

            _mockPasswordDigestRepository = new Mock<IPasswordDigestRepository>();
            
            _mockDataContext = new Mock<IDataContext>();
            _mockDataContext.Setup(dataContext => dataContext.ApplicationUserRepository)
                .Returns(_mockApplicationUserRepository.Object);
            _mockDataContext.Setup(dataContext => dataContext.PasswordDigestRepository)
                .Returns(_mockPasswordDigestRepository.Object);

            _userRepository = new PersistenceContext(_mockDataContext.Object).UserRepository;
        }

        [Test]
        public void ShouldMapReturnedUsers()
        {
            var applicationUser = CreateApplicationUser("Bruce", "Wayne", "Batman");
            _mockApplicationUserRepository.Setup(repository => repository.FindByUserName(applicationUser.UserName))
                .Returns(applicationUser);

            var actual = _userRepository.FindUserByUserName(applicationUser.UserName);

            AssertEquals(applicationUser, actual);
        }

        [Test]
        public void ShouldMapReturnedPasswords()
        {
            var applicationUser = CreateApplicationUser("Bruce", "Wayne", "Batman");
            var user = CreateUser("Bruce", "Wayne", "Batman");
            var passwordDigest = CreatePasswordDigest(applicationUser);
            _mockPasswordDigestRepository.Setup(repository => repository.FindByUserName(applicationUser.UserName))
                .Returns(passwordDigest);

            var actual = _userRepository.FindPasswordByUserName(applicationUser.UserName);

            AssertEquals(passwordDigest, actual, user);
        }

        [Test]
        public void ShouldReturnNullForMissingUsers()
        {
            const string userName = "missing";

            var found = _userRepository.FindUserByUserName(userName);

            Assert.IsNull(found);
        }

        [Test]
        public void ShouldCreateNewUser()
        {
            var user = CreateUser("Bruce", "Wayne", "Batman");
            var passwordHash = CreatePasswordHash();

            _userRepository.CreateNewUser(user, passwordHash);

            _mockApplicationUserRepository.Verify(repository =>
                repository.Add(It.Is<ApplicationUser>(entity => AssertEquals(entity, user)))
            );
            _mockPasswordDigestRepository.Verify(repository =>
                repository.Add(It.Is<PasswordDigest>(entity => AssertEquals(entity, passwordHash, user))));
        }

        [Test]
        public void ShouldThrowIfCreatingDuplicateUserName()
        {
            var lateUser = CreateUser("Clark", "Kent", "Batman");
            var passwordHash = CreatePasswordHash();
            _mockApplicationUserRepository.Setup(repository => repository.IsUserNameTaken(lateUser.UserName))
                .Returns(true);

            Assert.Throws<InvalidOperationException>(() => _userRepository.CreateNewUser(lateUser, passwordHash));
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
        public void ShouldDisposeRepositories()
        {
            _userRepository.Dispose();

            _mockApplicationUserRepository.Verify(repository => repository.Dispose());
            _mockPasswordDigestRepository.Verify(repository => repository.Dispose());
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

        private PasswordDigest CreatePasswordDigest(ApplicationUser applicationUser)
        {
            return new PasswordDigest
            {
                User = applicationUser,
                Cryptor = "cryptor",
                Digest = new byte[]{0x01, 0x02, 0x04},
                HashingAlgorithm = HashAlgorithm.SHA512.Name,
                Iterations = 5,
                SaltUsed = new byte[]{0x03, 0x02, 0x01},
            };
        }

        private static PasswordHash CreatePasswordHash()
        {
            return new PasswordHash
            {
                CryptorName = "cryptor",
                HashDigest = new byte[]{0x01, 0x02, 0x04},
                HashAlgorithm = HashAlgorithm.SHA512,
                Iterations = 5,
                Salt = new byte[]{0x03, 0x02, 0x01},
            };
        }

        private bool AssertEquals(ApplicationUser entity, User user)
        {
            Assert.AreEqual(entity.FirstName, user.FirstName);
            Assert.AreEqual(entity.LastName, user.LastName);
            Assert.AreEqual(entity.UserName, user.UserName);
            
            return true;
        }

        private bool AssertEquals(PasswordDigest entity, PasswordHash passwordHash, User user)
        {
            Assert.AreEqual(entity.Cryptor, passwordHash.CryptorName);
            Assert.AreEqual(entity.Digest, passwordHash.HashDigest);
            Assert.AreEqual(entity.HashingAlgorithm, passwordHash.HashAlgorithm.Name);
            Assert.AreEqual(entity.Iterations, passwordHash.Iterations);
            Assert.AreEqual(entity.SaltUsed, passwordHash.Salt);

            return AssertEquals(entity.User, user);
        }
    }
}
