using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sonneville.Investing.Persistence.EFCore
{
    public interface IEntityFrameworkRepository<TEntity, in TKey> : IDisposable where TEntity : class
    {
        TEntity Get(TKey id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        long Count();

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
