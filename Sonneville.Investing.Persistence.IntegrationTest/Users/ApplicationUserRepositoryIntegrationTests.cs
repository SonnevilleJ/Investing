using NUnit.Framework;
using Sonneville.Investing.Persistence.EFCore.EntityFrameworkCore;
using Sonneville.Investing.Persistence.EFCore.Users;

namespace Sonneville.Investing.Persistence.IntegrationTest.Users
{
    [TestFixture]
    public class ApplicationUserRepositoryIntegrationTests
        : BaseEntityFrameworkRepositoryTests<ApplicationUser, long, IApplicationUserRepository>
    {
        protected override IApplicationUserRepository InitializeRepository(IDataContext dbContext)
        {
            return dbContext.ApplicationUserRepository;
        }

        protected override ApplicationUser CreateEntity(int seed = 0)
        {
            return new ApplicationUser
            {
                FirstName = (seed * 1).ToString(),
                LastName = (seed * 2).ToString(),
                UserName = (seed * 3).ToString(),
            };
        }

        protected override void AssertEqual(ApplicationUser expected, ApplicationUser actual)
        {
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.UserName, actual.UserName);
        }

        [Test]
        public void ShouldFindUserByUserName()
        {
            var applicationUser = CreateEntity();

            Repository.Add(applicationUser);
            DbContext.SaveChanges();

            var user = Repository.FindByUserName(applicationUser.UserName);
            AssertEqual(applicationUser, user);
        }

        [Test]
        public void ShouldNotFindNonExistingUser()
        {
            var user = Repository.FindByUserName("unknown");
            Assert.IsNull(user);
        }

        [Test]
        public void ShouldReturnTrueIfUserNameTaken()
        {
            var applicationUser = CreateEntity();
            Repository.Add(applicationUser);
            DbContext.SaveChanges();

            var isUserNameTaken = Repository.IsUserNameTaken(applicationUser.UserName);

            Assert.IsTrue(isUserNameTaken);
        }

        [Test]
        public void ShouldReturnFalseIfUserNameAvailable()
        {
            var applicationUser = CreateEntity();

            var isUserNameTaken = Repository.IsUserNameTaken(applicationUser.UserName);

            Assert.IsFalse(isUserNameTaken);
        }

        [Test]
        public void ShouldReturnFalseIfUserNameTakenWithoutSave()
        {
            var applicationUser = CreateEntity();
            Repository.Add(applicationUser);

            var isUserNameTaken = Repository.IsUserNameTaken(applicationUser.UserName);

            Assert.IsFalse(isUserNameTaken);
        }
    }
}
