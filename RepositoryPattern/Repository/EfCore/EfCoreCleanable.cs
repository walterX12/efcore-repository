using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RepositoryPattern.Repository.DataCleaning;
using RepositoryPattern.Repository.EfCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryPattern.Repository.EfCore
{
    /// <summary>
    /// Enables to clean old data from the database without defining repository. Enables to clean DB table (withut defining entity like required in <see cref="EfCoreCleanable{TDbContext}"/>)
    /// </summary>
    /// <typeparam name="TDbContext">EF core DbContext</typeparam>
    public abstract class EfCoreCleanable<TDbContext> : IDataCleaner
        where TDbContext : DbContext
    {

        protected TDbContext context { get; private set; }

#pragma warning disable CA1051 // Do not declare visible instance fields
        protected ILogger logger;
#pragma warning restore CA1051 // Do not declare visible instance fields

        protected EfCoreCleanable(TDbContext context):this(context, new NullLogger<EfCoreCleanable<TDbContext>>())
        {
        }

        protected EfCoreCleanable(TDbContext context, ILogger logger)
        {
            this.context = context;
            this.logger = logger;
        }

        /// <summary>
        /// Deletes data in batches. For example if Oracle database is used, it is necessary to delete data in batches to avoid memory issues
        /// Uses <see cref="RemoveOlderOneBatchAsync(TimeSpan, int)"/> to delete single data batch
        /// </summary>
        /// <param name="retentionPeriod">How old data are kept. Older are deleted</param>
        /// <param name="batchSize"></param>
        /// <returns>Number of deleted items</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<int> RemoveOldAsync(TimeSpan retentionPeriod, int batchSize)
        {
            logger.LogInformation("RemoveOldAsync started for retentionPeriod {retentionPeriod} and batchSize {batchSize}", retentionPeriod, batchSize);
            int rows = 0;
            bool progressDeletion = true;
            int cycles = 0;

            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be greater than 0");
            }

            while (progressDeletion)
            {
                var deletedEntries = await RemoveOneBatchAsync(retentionPeriod, batchSize).ConfigureAwait(false);
                progressDeletion = deletedEntries > 0;
                cycles++;
                rows += deletedEntries;
            }
            logger.LogInformation("RemoveOldAsync finished deleted {rows} row(s) in {cycles} cycle(s) for retentionPeriod {retentionPeriod} and batchSize {batchSize}", rows, cycles, retentionPeriod, batchSize);
            return rows;
        }

        /// <summary>
        /// Deletes single data batch (of size <paramref name="take"/>). 
        /// </summary>
        /// <param name="retentionPeriod">How old data are kept. Older data are deleted</param>
        /// <param name="take"></param>
        /// <returns>Number of deleted items. Should be between zero and <paramref name="take"/></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public abstract Task<int> RemoveOneBatchAsync(TimeSpan retentionPeriod, int take);


        ///// <summary>
        ///// Deletes data (limited to <paramref name="batchSize"/> rows ).
        ///// </summary>
        ///// <param name="retentionPeriod">How old data are kept. Older are deleted</param>
        ///// <param name="batchSize">How many rows are deleted</param>
        ///// <returns>Number of deleted items</returns>
        ///// <exception cref="ArgumentOutOfRangeException">If batchSize is 0 or less or retentionPeriod is not valid</exception>
        //protected abstract Task<int> RemoveOlderOneBatchAsync(TimeSpan retentionPeriod, int batchSize);

        // Example
        /// <summary>
        /// Deletes data (limited to <paramref name="batchSize"/> rows ).
        /// </summary>
        /// <param name="retentionPeriod">How old data are kept. Older are deleted</param>
        /// <param name="batchSize">How many rows are deleted</param>
        /// <returns>Number of deleted items</returns>
        /// <exception cref="ArgumentOutOfRangeException">If batchSize is 0 or less or retentionPeriod is not valid</exception>
        //public virtual async Task<int> RemoveOlderOneBatchAsync(TimeSpan retentionPeriod, int batchSize)
        //{
        // IN EF Core 7 can be used https://learn.microsoft.com/en-us/ef/core/saving/execute-insert-update-delete
        // E.g. context.Blogs.Where(b => b.Rating < 3).ExecuteDelete();
        //    //logger.LogTrace($"RemoveOlderOneBatchAsync started for retentionPeriod {retentionPeriod} and batchSize {batchSize}");
        //    if (batchSize < 1) throw new ArgumentOutOfRangeException(nameof(batchSize));

        //    var rowsModified = await context.Database.ExecuteSqlRawAsync($"DELETE FROM [SpeechToTextEvents] " +
        //        $"WHERE [Timestamp] < {DateTime.Now - retentionPeriod} and" +
        //        $"ROWNUM <= {batchSize}").ConfigureAwait(false);

        //    logger.LogTrace($"RemoveOlderOneBatchAsync finished returning {rowsModified} for retentionPeriod {retentionPeriod} and batchSize {batchSize}");
        //    return rowsModified;
        //}

    }
}
