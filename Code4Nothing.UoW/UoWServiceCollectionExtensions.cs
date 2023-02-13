namespace Code4Nothing.UoW;

public static class UoWServiceCollectionExtensions
{
    public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services) where TContext : DbContext
    {
        services.AddScoped<IRepositoryFactory, UoW<TContext>>();
        // Following has a issue: IUnitOfWork cannot support multiple dbcontext/database, 
        // that means cannot call AddUnitOfWork<TContext> multiple times.
        // Solution: check IUnitOfWork whether or null
        services.AddScoped<IUoW, UoW<TContext>>();
        services.AddScoped<IUoW<TContext>, UoW<TContext>>();

        return services;
    }
    
    public static IServiceCollection AddCustomRepository<TEntity, TRepository>(this IServiceCollection services)
        where TEntity : class
        where TRepository : class, IRepository<TEntity>
    {
        services.AddScoped<IRepository<TEntity>, TRepository>();
        return services;
    }
}