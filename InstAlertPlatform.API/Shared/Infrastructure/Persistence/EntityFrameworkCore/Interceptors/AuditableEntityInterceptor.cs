using InstAlertPlatform.API.Shared.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
public sealed class AuditableEntityInterceptor(TimeProvider clock) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData data, InterceptionResult<int> result)
    {
        Apply(data.Context);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData data, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Apply(data.Context);
        return ValueTask.FromResult(result);
    }

    private void Apply(DbContext? context)
    {
        if (context is null)
            return;
        var now = clock.GetUtcNow();
        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified))
                continue;
            entry.Entity.UpdatedAt = now;
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt ??= now;
            var version = entry.Metadata.FindProperty(nameof(AuditedEntity.Version));
            if (version?.IsConcurrencyToken == true && version.ClrType == typeof(string))
                entry.Property(version.Name).CurrentValue = Guid.NewGuid().ToString();
            if (entry.State == EntityState.Modified)
            {
                var created = entry.Property(nameof(IAuditableEntity.CreatedAt));
                created.CurrentValue = created.OriginalValue;
                created.IsModified = false;
            }
        }
    }
}
