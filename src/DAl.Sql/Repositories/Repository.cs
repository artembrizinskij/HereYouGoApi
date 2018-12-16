using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DAl.Sql.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly CommonContext Context;

        public Repository(CommonContext context)
        {
            Context = context;
        }

        public IEnumerable<TEntity> FindMany(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> fetcher = null)
        {
            var query = Context.Set<TEntity>().Where(expression);
            if (fetcher != null)
                query = query.Include(fetcher);
            return query.ToArray();
        }

        public TEntity FindOne(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> fetcher = null)
        {
            var query = Context.Set<TEntity>().Where(expression);
            if (fetcher != null)
                query = query.Include(fetcher);
            return query.FirstOrDefault();
        }

        public TEntity FindOneById(Guid id, Expression<Func<TEntity, object>> fetcher = null)
        {
            var query = Context.Set<TEntity>().Where(x => x.Id == id);
            if (fetcher != null)
                query = query.Include(fetcher);
            return query.FirstOrDefault();
        }

        public void AddOne(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
        }

        public void AddMany(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                AddOne(entity);
            }
        }

        public void Update(TEntity entity)
        {
            Context.Update(entity);
        }

        public void UpdateMany(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Update(entity);
            }
        }
    }
}
