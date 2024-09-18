using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryPattern.Repository.DataCleaning
{
    /// <summary>
    /// Enables to delete old data from the database
    /// </summary>
    public interface IDataCleaner
        //<TEntity>
        // where TEntity : class, ITimestamped
    {
        ///// <summary>
        ///// Deletes data in batches. It is necessary to delete data in batches in case that huge number of rows is affected
        ///// </summary>
        ///// <param name="retentionPeriod">How old data are kept. Older data are deleted</param>
        ///// <param name="batchSize"></param>
        ///// <returns>Number of deleted items</returns>
        ///// <exception cref="ArgumentOutOfRangeException"></exception>
        //public Task<int> RemoveOldAsync(TimeSpan retentionPeriod, int batchSize, Func<TEntity, DateTime> timestampAccessor);

        /// <summary>
        /// Deletes data in batches. It is necessary to delete data in batches in case that huge number of rows is affected
        /// </summary>
        /// <param name="retentionPeriod">How old data are kept. Older data are deleted</param>
        /// <param name="batchSize"></param>
        /// <returns>Number of deleted items</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Task<int> RemoveOldAsync(TimeSpan retentionPeriod, int batchSize);

        /// <summary>
        /// Deletes single data batch (of size <paramref name="take"/>). 
        /// </summary>
        /// <param name="retentionPeriod">How old data are kept. Older data are deleted</param>
        /// <param name="take"></param>
        /// <returns>Number of deleted items. Should be between zero and <paramref name="take"/></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Task<int> RemoveOneBatchAsync(TimeSpan retentionPeriod, int take);

    }
}
