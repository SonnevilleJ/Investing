using NUnit.Framework;
using Sonneville.Investing.Persistence.EFCore.EntityFrameworkCore;
using Sonneville.Investing.Persistence.EFCore.Security;
using Sonneville.Investing.Persistence.EFCore.Users;

namespace Sonneville.Investing.Persistence.IntegrationTest.Security
{
    [TestFixture]
    public class PasswordDigestRepositoryIntegrationTests
        : BaseEntityFrameworkRepositoryTests<PasswordDigest, long, IPasswordDigestRepository>
    {
        protected override IPasswordDigestRepository InitializeRepository(IDataContext dbContext)
        {
            return dbContext.PasswordDigestRepository;
        }

        protected override PasswordDigest CreateEntity(int seed = 0)
        {
            return new PasswordDigest
            {
                User = new ApplicationUser
                {
                    FirstName = (seed * 5).ToString(),
                    LastName = (seed * 6).ToString(),
                    UserName = (seed * 7).ToString(),
                },
                Cryptor = (seed * 1).ToString(),
                Digest = new[] {byte.MinValue, (byte) seed, byte.MaxValue},
                HashingAlgorithm = (seed * 2).ToString(),
                Iterations = seed,
                SaltUsed = new[] {byte.MaxValue, (byte) seed, byte.MinValue}
            };
        }

        protected override void AssertEqual(PasswordDigest expected, PasswordDigest actual)
        {
            Assert.AreEqual(expected.Cryptor, actual.Cryptor);
            Assert.AreEqual(expected.Digest, actual.Digest);
            Assert.AreEqual(expected.HashingAlgorithm, actual.HashingAlgorithm);
            Assert.AreEqual(expected.Iterations, actual.Iterations);
            Assert.AreEqual(expected.SaltUsed, actual.SaltUsed);
            Assert.AreEqual(expected.User, actual.User);
            Assert.AreEqual(expected.UserDatabaseId, actual.UserDatabaseId);
        }

        [Test]
        public void ShouldFindByUserName()
        {
            var passwordDigest = CreateEntity();
            Repository.Add(passwordDigest);
            DbContext.SaveChanges();

            var actual = Repository.FindByUserName(passwordDigest.User.UserName);

            AssertEqual(passwordDigest, actual);
        }

        [Test]
        public void ShouldNotFindNonExistingPassword()
        {
            var actual = Repository.FindByUserName("bob");

            Assert.IsNull(actual);
        }

        [Test]
        public void ShouldOverwriteRatherThanStoreMultiplePasswordsPerUser()
        {
            // This seems to be a feature of EF Core:
            // Given an object A1 is persisted with a foreign key relation to B
            // When calling Add() with A2 with the same relation to B
            // Then the existing object will be overwritten
            // I expected it to fail due to a unique key constraint

            var password1 = CreateEntity(1);
            Repository.Add(password1);
            DbContext.SaveChanges();

            var password2 = CreateEntity(2);
            password2.User = password1.User;
            Repository.Add(password2);
            DbContext.SaveChanges();

            AssertEqual(password2, Repository.FindByUserName(password2.User.UserName));
        }
    }
}
