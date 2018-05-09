using System;
using Moq;
using NUnit.Framework;
using Sonneville.Investing.Persistence.EFCore.EntityFrameworkCore;

namespace Sonneville.Investing.Persistence.Test
{
    [TestFixture]
    public class PersistenceContextTests
    {
        private IPersistenceContext _persistenceContext;
        private Mock<IDataContext> _mockDataContext;

        [SetUp]
        public void Setup()
        {
            _mockDataContext = new Mock<IDataContext>();
            
            _persistenceContext = new PersistenceContext(_mockDataContext.Object);
        }

        [Test]
        public void ShouldSave()
        {
            _mockDataContext.Setup(context => context.SaveChanges()).Returns(5);
            
            var result = _persistenceContext.SaveChanges();

            Assert.AreEqual(5, result);
        }

        [Test]
        public void ShouldDispose()
        {
            var disposable = (IDisposable) _persistenceContext;
            Assert.IsTrue(disposable != null);
            disposable.Dispose();

            _mockDataContext.Verify(context => context.Dispose());
        }
    }
}
