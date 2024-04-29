using Microsoft.EntityFrameworkCore;
using Repository;
using System.Linq.Expressions;

namespace RepositoryPattern.Repository.EfCore
{
    /// <summary>
    /// Ef core immplementation of <see cref="IRepository{TEntity, TEntityId}"/>
    /// </summary>
    /// <typeparam name="TEntity">Type of Entity</typeparam>
    /// <typeparam name="TEntityId">Entity identifier (typocally int or string)</typeparam>
    /// <typeparam name="TDbContext">EF core DbContext</typeparam>
    public class RepositoryEfCore<TEntity, TDbContext> : IRepository<TEntity>
        where TEntity : class
        where TDbContext : DbContext
    {
        private readonly TDbContext context;
        private bool disposed;

        public RepositoryEfCore(TDbContext context)
        {
            this.context = context;
        }

        public void Add(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);
            context.SaveChanges();
        }

        public void AddRange(IEnumerable<TEntity> entity)
        {
            context.Set<TEntity>().AddRange(entity);
            context.SaveChanges();
        }


//#if NET5_0_OR_GREATER
        public TEntity? GetById<TEntityId>(TEntityId id)  where TEntityId : struct
        {
            return context.Set<TEntity>().Find(id);
        }

        public async Task<TEntity?> GetByIdAsync<TEntityId>(TEntityId id)  where TEntityId : struct
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

        //#else

        //        public TEntity GetById<TEntityId>(TEntityId id)  where TEntityId : struct
        //        {
        //            return context.Set<TEntity>().Find(id);
        //        }

        //        public async Task<TEntity> GetByIdAsync<TEntityId>(TEntityId id)  where TEntityId : struct
        //        {
        //            return await context.Set<TEntity>().FindAsync(id).ConfigureAwait(false);
        //        }

        //        public TEntity Find(Expression<Func<TEntity, bool>> predicate)
        //        {
        //            return context.Set<TEntity>().FirstOrDefault(predicate);
        //        }

        //        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate)
        //        {
        //            return await context.Set<TEntity>().FirstOrDefaultAsync(predicate).ConfigureAwait(false);
        //        }
        //#endif

        public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate)
        {
            return context.Set<TEntity>().Where(predicate).ToList();
        }

        public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Task.Run(() => context.Set<TEntity>().Where(predicate)).ConfigureAwait(false);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return context.Set<TEntity>().ToList();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Task.Run(() => context.Set<TEntity>()).ConfigureAwait(false);
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

        public void Remove(TEntity entity)
        {
            context.Set<TEntity>().Remove(entity);
            context.SaveChanges();
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

