using System;
using System.Linq;
using NUnit.Framework;
using Sonneville.Investing.Persistence.EFCore;
using Sonneville.Investing.Persistence.EFCore.EntityFrameworkCore;

namespace Sonneville.Investing.Persistence.IntegrationTest
{
    [TestFixture]
    public abstract class BaseEntityFrameworkRepositoryTests<TEntity, TKey, TRepository>
        where TEntity : Entity<TKey>
        where TRepository : IEntityFrameworkRepository<TEntity, TKey>
    {
        protected DataContext DbContext;
        protected TRepository Repository;

        protected virtual DataContext InitializeDbContext()
        {
            return IntegrationTestConnection.GetDataContext();
        }

        protected abstract TRepository InitializeRepository(IDataContext dbContext);

        protected abstract TEntity CreateEntity(int seed = 0);

        protected abstract void AssertEqual(TEntity expected, TEntity actual);

        [SetUp]
        public virtual void Setup()
        {
            DbContext = InitializeDbContext();

            DbContext.Database.EnsureDeleted();
            DbContext.Database.EnsureCreated();

            Repository = InitializeRepository(DbContext);
        }

        [TearDown]
        public void Teardown()
        {
            InitializeDbContext().Database.EnsureDeleted();
        }

        [Test]
        public void ShouldAddAndGet()
        {
            var entity = CreateEntity();

            Repository.Add(entity);
            DbContext.SaveChanges();

            var found = Repository.Get(entity.DatabaseId);
            AssertEqual(entity, found);
        }

        [Test]
        public void ShouldAddRangeAndGet()
        {
            var entity = CreateEntity();

            Repository.AddRange(new[] {entity});
            DbContext.SaveChanges();

            var found = Repository.GetAll();
            AssertEqual(entity, found.Single());
        }

        [Test]
        public void ShouldAddAndFindById()
        {
            var entity = CreateEntity();

            Repository.Add(entity);
            DbContext.SaveChanges();

            var found = Repository.Find(e => Equals(e.DatabaseId, entity.DatabaseId));
            AssertEqual(entity, found.Single());
        }

        [Test]
        public void ShouldNotCountEntityBeforeSave()
        {
            Assert.AreEqual(0, Repository.Count());

            Repository.Add(CreateEntity());
            Assert.AreEqual(0, Repository.Count());

            DbContext.SaveChanges();
            Assert.AreEqual(1, Repository.Count());
        }

        [Test]
        public void ShouldNotCountRemovedEntityAfterSave()
        {
            var entity = CreateEntity();
            Repository.Add(entity);
            DbContext.SaveChanges();

            Repository.Remove(entity);
            DbContext.SaveChanges();

            Assert.AreEqual(0, Repository.Count());
            Assert.AreNotEqual(default(TKey), entity.DatabaseId); // EF Core maintains ID on deleted entities
        }

        [Test]
        public void ShouldCountRemovedEntityBeforeSave()
        {
            var entity = CreateEntity();
            Repository.Add(entity);
            DbContext.SaveChanges();

            Repository.Remove(entity);

            Assert.AreEqual(1, Repository.Count());
            Assert.AreNotEqual(default(TKey), entity.DatabaseId);
        }

        [Test]
        public void ShouldNotStoreIdWhenAddAndRemoveBeforeSave()
        {
            var entity = CreateEntity();

            Repository.Add(entity);
            Repository.Remove(entity);
            DbContext.SaveChanges();

            Assert.AreEqual(0, Repository.Count());
            Assert.AreEqual(default(TKey), entity.DatabaseId); // never received an ID
        }

        [Test]
        public void ShouldNotCountRemovedEntitiesAfterSave()
        {
            var entity = CreateEntity();
            Repository.Add(entity);
            DbContext.SaveChanges();

            Repository.RemoveRange(new[] {entity});
            DbContext.SaveChanges();

            Assert.AreEqual(0, Repository.Count());
            Assert.AreNotEqual(default(TKey), entity.DatabaseId); // EF Core maintains ID on deleted entities
        }

        [Test]
        public void ShouldCountRemovedEntitiesBeforeSave()
        {
            var entity = CreateEntity();
            Repository.Add(entity);
            DbContext.SaveChanges();

            Repository.RemoveRange(new[] {entity});

            Assert.AreEqual(1, Repository.Count());
            Assert.AreNotEqual(default(TKey), entity.DatabaseId);
        }

        [Test]
        public void ShouldNotStoreIdWhenAddAndRemoveRangeBeforeSave()
        {
            var entity = CreateEntity();

            Repository.Add(entity);
            Repository.RemoveRange(new[] {entity});
            DbContext.SaveChanges();

            Assert.AreEqual(0, Repository.Count());
            Assert.AreEqual(default(TKey), entity.DatabaseId); // never received an ID
        }

        [Test]
        public void ShouldDisposeContext()
        {
            void Code()
            {
                var dbContextDatabase = DbContext.Database;
            }

            Assert.DoesNotThrow(Code);

            Repository.Dispose();

            Assert.Throws<ObjectDisposedException>(Code);
        }
    }
}