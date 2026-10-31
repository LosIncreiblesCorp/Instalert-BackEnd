using InstAlertPlatform.API.Shared.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public class BaseRepository<TEntity>(AppDbContext context) : IBaseRepository<TEntity> where TEntity : class
{
    protected readonly AppDbContext Context = context;
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public async Task<TEntity?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var keyType = Context.Model.FindEntityType(typeof(TEntity))!.FindPrimaryKey()!.Properties.Single().ClrType;
        if (keyType == typeof(int))
            return int.TryParse(id, out var number) ? await Context.Set<TEntity>().FindAsync([number], cancellationToken) : null;
        return await Context.Set<TEntity>().FindAsync([id], cancellationToken);
    }

    public async Task<TEntity?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        => await Context.Set<TEntity>().FindAsync([id], cancellationToken);

    public void Update(TEntity entity)
    {
        Context.Set<TEntity>().Update(entity);
    }

    public void Remove(TEntity entity)
    {
        Context.Set<TEntity>().Remove(entity);
    }

    public async Task<IEnumerable<TEntity>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>().ToListAsync(cancellationToken);
    }
}
