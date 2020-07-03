using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.DataAccess.Repositories
{
    public class EntityFrameworkCoreRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
    {
        protected DbSet<TEntity> DBSet => this.DbContext.Set<TEntity>();

        protected IQueryable<TEntity> Queryable => this.DBSet.AsQueryable();

        protected DbContext DbContext { get; private set; }

        protected EntityFrameworkCoreRepository() { }

        protected EntityFrameworkCoreRepository(DbContext dbContext)
        {
            this.SetContext(dbContext);
        }

        public void SetContext(DbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        #region IRepositoryRealization
        public virtual TEntity GetById(TKey id)
        {
            return this.DBSet.Find(id);
        }

        public Task<TEntity> GetByIdAsync(TKey id)
        {
            return this.DBSet.FindAsync(id).AsTask();
        }

        public virtual ICollection<TEntity> GetAll()
        {
            return this.DBSet.ToList();
        }

        public virtual async Task<ICollection<TEntity>> GetAllAsync()
        {
            return await this.DBSet.ToListAsync();
        }

        public virtual ICollection<TEntity> Find(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null)
        {
            return this.GenericFindQuery(filter, orderBy, includes).ToList();
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return await this.DBSet.AnyAsync(filter);
        }

        public virtual async Task<ICollection<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null
            )
        {
            List<TEntity> result = await this.GenericFindQuery(filter, orderBy, includes).ToListAsync();
            return result;
        }

        public ICollection<TResult> FindAndMap<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null)
        {
            return this.GenericFindQuery(filter, orderBy, includes).Select(selector).ToList();
        }

        public async Task<ICollection<TResult>> FindAndMapAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null)
        {
            return await this.GenericFindQuery(filter, orderBy, includes).Select(selector).ToListAsync();
        }

        public async Task<TResult> FindByKeyAndMapAsync<TResult>(
           Expression<Func<TEntity, bool>> keyFilter,
           Expression<Func<TEntity, TResult>> selector)
        {
            return await this.GenericFindQuery(keyFilter, null, null).Select(selector).FirstOrDefaultAsync();
        }

        public void Add(TEntity entity)
        {
            this.DBSet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            this.DBSet.AddRange(entities);
        }

        public void Update(TEntity entity)
        {
            this.DBSet.Attach(entity);
            this.DbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(TEntity entity)
        {
            this.DBSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            this.DBSet.RemoveRange(entities);
        }

        private IQueryable<TEntity> GenericFindQuery(
        Expression<Func<TEntity, bool>> filter,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
        IEnumerable<Expression<Func<TEntity, object>>> includes)
        {
            IQueryable<TEntity> query = this.Queryable;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }

            return query;
        }
        #endregion IRepositoryRealization
    }
}
