using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Domain.Common;

namespace RestaurantSystem.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor that implements soft delete for entities
/// </summary>
public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    private readonly IDateTime _dateTime;

    public SoftDeleteInterceptor(IDateTime dateTime)
    {
        _dateTime = dateTime;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State == EntityState.Deleted)
            {
                // Instead of deleting, mark as deleted
                entry.State = EntityState.Modified;
                entry.Entity.DeletedAt = _dateTime.UtcNow;
            }
        }
    }
}