namespace Code4Nothing.UoW;

public interface IUoW : IDisposable
{
    void ChangeDatabase(string database);

    IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = false) where TEntity : class;

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    
    void TrackGraph(object rootEntity, Action<EntityEntryGraphNode> callback);
    
    // int ExecuteRawSqlCommand(string sql, params object[] parameters);
    // IQueryable<TEntity> FromRawSql<TEntity>(string sql, params object[] parameters) where TEntity : class;
}

public interface IUoW<TContext> : IUoW where TContext : DbContext
{
    TContext DbContext { get; }

    Task<int> SaveChangesAsync(params IUoW[] uoWs);
} 