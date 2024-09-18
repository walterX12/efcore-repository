using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Repository;
using System.Linq.Expressions;

namespace RepositoryPattern.Repository.EfCore
{
    /// <summary>
    /// Ef core implementation of <see cref="IRepository{TEntity, TEntityId}"/>
    /// </summary>
    /// <typeparam name="TEntity">Type of Entity</typeparam>
    /// <typeparam name="TDbContext">EF core DbContext</typeparam>
    public class RepositoryEfCore<TEntity, TDbContext> : IRepository<TEntity>
        where TEntity : class
        where TDbContext : DbContext
    {
        //private readonly TDbContext context;

        protected TDbContext context { get; private set ; }

        private bool disposed;

        /// <summary>
        /// Library can be used without logger. In this case, logging is disabled.
        /// </summary>
#pragma warning disable CA1051 // Do not declare visible instance fields
        protected ILogger logger;
#pragma warning restore CA1051 // Do not declare visible instance fields

        public RepositoryEfCore(TDbContext context)
        {
            this.context = context;
            logger = new NullLogger<RepositoryEfCore<TEntity, TDbContext>>();
        }

        //public RepositoryEfCore(TDbContext context): this(context, new NullLogger<RepositoryEfCore<TEntity, TDbContext>>())
        //{
        //}

        public RepositoryEfCore(TDbContext context, ILogger logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public void Add(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);
            context.SaveChanges();
        }

        public async Task AddAsync(TEntity entity)
        {
            await context.Set<TEntity>().AddAsync(entity).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public void AddRange(IEnumerable<TEntity> entity)
        {
            context.Set<TEntity>().AddRange(entity);
            context.SaveChanges();
        }
//#if NET5_0_OR_GREATER
        public TEntity? GetById<TEntityId>(TEntityId id) where TEntityId : struct
        {
            return context.Set<TEntity>().Find(id);
        }

        public async Task<TEntity?> GetByIdAsync<TEntityId>(TEntityId id) where TEntityId : struct
        {
            return await context.Set<TEntity>().FindAsync(id).ConfigureAwait(false);
        }

        public TEntity? Find(Expression<Func<TEntity, bool>> predicate)
        {
            return context.Set<TEntity>().FirstOrDefault(predicate);
        }

        public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await context.Set<TEntity>().FirstOrDefaultAsync(predicate).ConfigureAwait(false);
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return context.Set<TEntity>().Count(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await context.Set<TEntity>().CountAsync(predicate).ConfigureAwait(false);
        }

        public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate)
        {
            return context.Set<TEntity>().Where(predicate).ToList();
        }

        public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await context.Set<TEntity>().Where(predicate).ToListAsync().ConfigureAwait(false);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return context.Set<TEntity>().ToList();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await context.Set<TEntity>().ToListAsync().ConfigureAwait(false);
        }

        public int Count()
        {
            return context.Set<TEntity>().Count();
        }

        public async Task<int> CountAsync()
        {
            return await context.Set<TEntity>().CountAsync().ConfigureAwait(false);
        }

        public void Update(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public void Remove(TEntity entity)
        {
            context.Set<TEntity>().Remove(entity);
            context.SaveChanges();
        }

        public int RemoveRange(IEnumerable<TEntity> entities)
        {
            context.Set<TEntity>().RemoveRange(entities);
            return context.SaveChanges();
        }

        public int RemoveRange(IQueryable<TEntity> entities)
        {
            context.Set<TEntity>().RemoveRange(entities);
            return context.SaveChanges();
        }

        public async Task<int> RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            context.Set<TEntity>().RemoveRange(entities);
            return await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public int Remove(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = context.Set<TEntity>().Where(predicate);
            context.Set<TEntity>().RemoveRange(entities);
            return context.SaveChanges();
        }

        /// <summary>
        /// Remove defined entities by predicate, but limit the number of deleted entities by take parameter
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        protected virtual async Task<int> RemoveAsync(Expression<Func<TEntity, bool>> predicate, int? take)
        {
            var entitiesBatchQueryable = context.Set<TEntity>().Where(predicate);
            if (take != null) entitiesBatchQueryable = entitiesBatchQueryable.Take(take.Value);
            var entitiesBatch = entitiesBatchQueryable.ToList();
            await RemoveRangeAsync(entitiesBatch).ConfigureAwait(false);
            return await context.SaveChangesAsync().ConfigureAwait(false);

        }

        public async Task<int> RemoveAsync(Expression<Func<TEntity, bool>> predicate)
        {
            //TODO optimalize using ExecuteDeleteAsync()
            //var entities = await context!.Set<TEntity>()
            //    .Where(predicate)
            //    .ExecuteDeleteAsync()   // Only supported in EF Core 7.0 and if imported <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
            //    .ConfigureAwait(false);
            //return await context.SaveChangesAsync().ConfigureAwait(false);
            return await RemoveAsync(predicate,null).ConfigureAwait(false);
        }

#if NET7_0_OR_GREATER
#else

        //public void RemoveWhere<T>(Expression<Func<T, bool>> filter) where T : class
        //{
        //    string selectSql = context.Set<T>().Where(filter).ToQueryString();
        //    int fromIndex = selectSql.IndexOf("FROM", StringComparison.InvariantCulture);
        //    int whereIndex = selectSql.IndexOf("WHERE", StringComparison.InvariantCulture);

        //    string fromSql = selectSql.Substring(fromIndex, whereIndex - fromIndex);
        //    string whereSql = selectSql.Substring(whereIndex);
        //    string aliasSQl = fromSql.IndexOf(" AS ", StringComparison.InvariantCulture) > -1 ? fromSql.Substring(fromSql.IndexOf(" AS ", StringComparison.InvariantCulture) + 4) : "";
        //    string deleteSql = string.Join(" ", "DELETE ", aliasSQl.Trim(), fromSql.Trim(), whereSql.Trim());
        //    context.Database.Exex(deleteSql);
        //}
#endif

        /// <summary>
        /// Deletes data in batches. For example if Oracle database is used, it is necessary to delete data in batches to avoid memory issues
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="batchSize"></param>
        /// <returns>Number of deleted items</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public virtual async Task<int> RemoveInBatchAsync( Expression<Func<TEntity, bool>> predicate, int batchSize) 
        {
            int result = 0;
            bool progressDeletion = true;

            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be greater than 0");
            }

            while (progressDeletion)
            {
                    var deletedEntries = await RemoveAsync(predicate,batchSize).ConfigureAwait(false);
                    progressDeletion = deletedEntries > 0;
                    result += deletedEntries;
            }
            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


    }
}

