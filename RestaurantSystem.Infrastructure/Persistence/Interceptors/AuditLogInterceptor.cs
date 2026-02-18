using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Domain.Common;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor that writes audit logs for inserts/updates/deletes.
/// </summary>
public class AuditLogInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly List<AuditEntry> _pendingAuditEntries = new();
    private bool _suppress;

    public AuditLogInterceptor(
        ICurrentUserService currentUserService,
        IDateTime dateTime,
        IHttpContextAccessor httpContextAccessor)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
        _httpContextAccessor = httpContextAccessor;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (!_suppress)
        {
            AddAuditEntries(eventData.Context);
        }
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (!_suppress)
        {
            AddAuditEntries(eventData.Context);
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        if (!_suppress)
        {
            SavePendingAuditEntries(eventData.Context);
        }
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (!_suppress)
        {
            await SavePendingAuditEntriesAsync(eventData.Context, cancellationToken);
        }
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private void AddAuditEntries(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog)
                continue;

            if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
                continue;

            var action = GetAction(entry);
            var auditEntry = new AuditEntry(entry, action);

            foreach (var property in entry.Properties)
            {
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyProperty = property;
                    continue;
                }

                if (entry.State == EntityState.Added)
                {
                    auditEntry.NewValues[property.Metadata.Name] = property.CurrentValue;
                }
                else if (entry.State == EntityState.Deleted || action == "DELETE")
                {
                    auditEntry.OldValues[property.Metadata.Name] = property.OriginalValue;
                }
                else if (entry.State == EntityState.Modified && property.IsModified)
                {
                    auditEntry.OldValues[property.Metadata.Name] = property.OriginalValue;
                    auditEntry.NewValues[property.Metadata.Name] = property.CurrentValue;
                }
            }

            if (auditEntry.HasTemporaryProperties)
            {
                _pendingAuditEntries.Add(auditEntry);
            }
            else
            {
                var log = CreateAuditLog(auditEntry);
                context.Set<AuditLog>().Add(log);
            }
        }
    }

    private void SavePendingAuditEntries(DbContext? context)
    {
        if (context == null || _pendingAuditEntries.Count == 0)
            return;

        _suppress = true;
        try
        {
            foreach (var auditEntry in _pendingAuditEntries)
            {
                var log = CreateAuditLog(auditEntry);
                context.Set<AuditLog>().Add(log);
            }

            _pendingAuditEntries.Clear();
            context.SaveChanges();
        }
        finally
        {
            _suppress = false;
        }
    }

    private async Task SavePendingAuditEntriesAsync(DbContext? context, CancellationToken cancellationToken)
    {
        if (context == null || _pendingAuditEntries.Count == 0)
            return;

        _suppress = true;
        try
        {
            foreach (var auditEntry in _pendingAuditEntries)
            {
                var log = CreateAuditLog(auditEntry);
                context.Set<AuditLog>().Add(log);
            }

            _pendingAuditEntries.Clear();
            await context.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            _suppress = false;
        }
    }

    private AuditLog CreateAuditLog(AuditEntry entry)
    {
        var userId = _currentUserService.UserId;
        var httpContext = _httpContextAccessor.HttpContext;
        var ip = httpContext?.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContext?.Request.Headers.UserAgent.ToString();

        return new AuditLog
        {
            TableName = entry.TableName,
            RecordId = entry.RecordId,
            Action = entry.Action,
            OldValues = entry.OldValues.Count == 0 ? null : JsonSerializer.Serialize(entry.OldValues),
            NewValues = entry.NewValues.Count == 0 ? null : JsonSerializer.Serialize(entry.NewValues),
            ChangedById = userId,
            ChangedByType = userId.HasValue ? "user" : "system",
            IpAddress = ip,
            UserAgent = userAgent,
            CreatedAt = _dateTime.UtcNow
        };
    }

    private static string GetAction(EntityEntry entry)
    {
        if (entry.State == EntityState.Deleted)
            return "DELETE";

        if (entry.State == EntityState.Added)
            return "INSERT";

        if (entry.Entity is ISoftDeletable softDeletable &&
            entry.State == EntityState.Modified &&
            softDeletable.DeletedAt.HasValue)
        {
            return "DELETE";
        }

        return "UPDATE";
    }

    private sealed class AuditEntry
    {
        public AuditEntry(EntityEntry entry, string action)
        {
            Entry = entry;
            Action = action;
            TableName = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name;
        }

        public EntityEntry Entry { get; }
        public PropertyEntry? KeyProperty { get; set; }
        public string TableName { get; }
        public string Action { get; }
        public Dictionary<string, object?> OldValues { get; } = new();
        public Dictionary<string, object?> NewValues { get; } = new();

        public bool HasTemporaryProperties
        {
            get
            {
                return Entry.Properties.Any(p => p.IsTemporary);
            }
        }

        public int RecordId
        {
            get
            {
                var keyValue = KeyProperty?.CurrentValue;
                return keyValue is int id ? id : 0;
            }
        }
    }
}
