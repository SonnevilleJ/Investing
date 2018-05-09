using System;
using Sonneville.Investing.Persistence.EFCore.EntityFrameworkCore;
using Sonneville.Investing.Persistence.Users;

namespace Sonneville.Investing.Persistence
{
    public interface IPersistenceContext : IDisposable
    {
        int SaveChanges();
    }

    public class PersistenceContext : IPersistenceContext
    {
        private readonly IDataContext _dataContext;

        public PersistenceContext(IDataContext dataContext)
        {
            _dataContext = dataContext;
            UserRepository = new UserRepository(_dataContext);
        }

        public IUserRepository UserRepository { get; }

        public int SaveChanges()
        {
            return _dataContext.SaveChanges();
        }

        public void Dispose()
        {
            _dataContext?.Dispose();
        }
    }
}
