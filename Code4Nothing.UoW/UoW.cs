namespace Code4Nothing.UoW;

public class UoW<TContext> : IRepositoryFactory, IUoW<TContext> where TContext: DbContext
{
    private readonly TContext _context;
    private bool _disposed = false;
    private Dictionary<Type, object>? _repositories;

    public TContext DbContext => _context;

    public UoW(TContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

    public void ChangeDatabase(string database)
    {
        var connection = _context.Database;
        // TODO
    }

    IRepository<TEntity> IUoW.GetRepository<TEntity>(bool hasCustomRepository)
    {
        _repositories ??= new Dictionary<Type, object>();

        if (hasCustomRepository)
        {
            var customRepository = _context.GetService<IRepository<TEntity>>();
            return customRepository;
        }

        var type = typeof(TEntity);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new Repository<TEntity>(_context);
        }

        return (IRepository<TEntity>)_repositories[type];
    }

    public int SaveChanges() => _context.SaveChanges();
    public async Task<int> SaveChangesAsync(CancellationToken ct = default) => await _context.SaveChangesAsync(ct);

    public void TrackGraph(object rootEntity, Action<EntityEntryGraphNode> callback) => _context.ChangeTracker.TrackGraph(rootEntity, callback);

    IRepository<TEntity> IRepositoryFactory.GetRepository<TEntity>(bool hasCustomRepository)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing) _repositories?.Clear();
            
            _context.Dispose();
        }

        _disposed = true;
    }

    public async Task<int> SaveChangesAsync(params IUoW[] uoWs)
    {
        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        var count = 0;
        foreach (var uow in uoWs)
        {
            count += await uow.SaveChangesAsync().ConfigureAwait(false);
        }

        count += await SaveChangesAsync();

        ts.Complete();

        return count;
    }
}