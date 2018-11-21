using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Domain.Abstractions;

namespace DAl.Sql.Repositories
{
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        IEnumerable<TEntity> FindMany(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> fetcher = null);

        TEntity FindOneById(Guid id, Expression<Func<TEntity, object>> fetcher = null);

        TEntity FindOne(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> fetcher = null);

        void AddOne(TEntity entity);

        void AddMany(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        void UpdateMany(IEnumerable<TEntity> entities);
    }
}