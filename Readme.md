# Repository pattern

Repository pattern implementation focused on EF Core.

## Getting started

### Add package to the project
```
dotnet add package RepositoryPatternEfCoreSimple
```

### Create a repository
```csharp
public class MyEntityRepository : RepositoryEfCore<MyEntity, MyDbContext>
{
	public MyEntityRepository(MyDbContext context) : base(context)
	{
	}

    public MyEntityRepository(MyDbContext context, ILogger<MyEntityRepository> logger) : base(context, logger)
    {
    }

}
```

### Use the repository
```csharp

	var repository = new MyEntityRepository(context); // or use DI

	// Add syncronously new entity
	repository.Add(new MyEntity());	// This will add the entity to the context and saves the changes

	// Add asyncronously new entity
	await repository.AddAsync(new MyEntity()).ConfigureAwait(false) ;	// This will add the entity to the context and saves the changes asyncronously

```


## Getting started when is needded regularly delete old data


### Add package to the project
```
dotnet add package RepositoryPatternEfCoreSimple
```

### Create a repository
```csharp
public class MyEntityRepository : RepositoryEfCoreCleanable<MyEntity, MyDbContext>
{
	public MyEntityRepository(MyDbContext context) : base(context)
	{
	}

    public MyEntityRepository(MyDbContext context, ILogger<MyEntityRepository> logger) : base(context, logger)
    {
    }

    /// <summary>
    /// Deletes single data batch (of size <paramref name="take"/>). 
    /// Example for Oracle
    /// </summary>
    /// <param name="retentionPeriod">How old data are kept. Older data are deleted</param>
    /// <param name="take"></param>
    /// <returns>Number of deleted items. Should be between zero and <paramref name="take"/></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async override Task<int> RemoveOneBatchAsync(TimeSpan retentionPeriod, int take)
    {
        logger.LogTrace($"RemoveOneBatchAsync started for retentionPeriod {retentionPeriod} and take {take}");
        if (take < 1) throw new ArgumentOutOfRangeException(nameof(take));
        var deletedRows = 0;
        var olderThan = DateTime.Now - retentionPeriod;
        //string command = "DELETE FROM MY_TABLE WHERE timestamp < :date"; // replace MY_TABLE by your table name
        deletedRows = context.Database.ExecuteSqlRaw("DELETE FROM MY_TABLE where ROWNUM <= {0} and  timestamp < {1}", take, olderThan);
        logger.LogTrace($"RemoveOneBatchAsync finished deleted {deletedRows} for retentionPeriod {retentionPeriod} and take {take}");
        return deletedRows;
    }

}
```

### Use the repository
```csharp

	var repository = new MyEntityRepository(context); // or use DI

	// Delete old data
    TimeSpan dataRetentionPeriod = TimeSpan a = TimeSpan.FromDays(30);      // Delete data older than 30 days
    int batchSize => 1000;      // Delete data in batches . No more tha 1000 rows are deleted in one DB transaction

    repository.RemoveOldAsync(dataRetentionPeriod, batchSize).ConfigureAwait(false);

```

## Versions

### 0.0.5
Improved logging to support parameters

### 0.0.4
Added documetation

### 0.0.3
Initial version with `RepositoryEfCoreCleanable` implementation

### 0.0.0-alpha4
Added `Count(expresion)` and `CountAsync(expresion)`

### 0.0.0-alpha3
Removed TId and Indetifyiable interface. Now the repository is generic and the key is defined by the entity.

### 0.0.0-alpha2
Initial version with `RepositoryEfCore` implementation