using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    /// <summary>
    /// Repository storing entities of type <see cref="TEntity"/> and this entities have an identifier of type <see cref="TEntityId"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TEntityId"></typeparam>
    public interface IRepository<TEntity> : IDisposable
        where TEntity : class
    {
        void Add(TEntity entity);

        void AddRange(IEnumerable<TEntity> entity);

//#if NET5_0_OR_GREATER
        TEntity? Find(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity? GetById<TEntityId>(TEntityId id)  where TEntityId : struct;

        Task<TEntity?> GetByIdAsync<TEntityId>(TEntityId id)  where TEntityId : struct;

        int Count(Expression<Func<TEntity, bool>> predicate);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);


        //#else
        //        TEntity Find(Expression<Func<TEntity, bool>> predicate);

        //        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate);

        //        TEntity GetById<TEntityId>(TEntityId id)  where TEntityId : struct;

        //        Task<TEntity> GetByIdAsync<TEntityId>(TEntityId id)  where TEntityId : struct;
        //#endif


        IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> GetAll();

        Task<IEnumerable<TEntity>> GetAllAsync();

        int Count();

        Task<int> CountAsync();

        void Update(TEntity entity);

        void Remove(TEntity entity);



    }
}
