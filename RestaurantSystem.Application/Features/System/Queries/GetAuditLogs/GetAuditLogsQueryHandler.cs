using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Application.Features.System.DTOs;

namespace RestaurantSystem.Application.Features.System.Queries.GetAuditLogs;

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, PaginatedList<AuditLogDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAuditLogsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<AuditLogDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.AuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Table))
        {
            query = query.Where(a => a.TableName == request.Table);
        }

        if (request.UserId.HasValue)
        {
            query = query.Where(a => a.ChangedById == request.UserId.Value);
        }

        if (request.From.HasValue)
        {
            query = query.Where(a => a.CreatedAt >= request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.Where(a => a.CreatedAt <= request.To.Value);
        }

        query = query.OrderByDescending(a => a.CreatedAt);

        var projected = query.Select(a => new AuditLogDto
        {
            Id = a.AuditId,
            TableName = a.TableName,
            RecordId = a.RecordId,
            Action = a.Action,
            OldValues = a.OldValues,
            NewValues = a.NewValues,
            UserId = a.ChangedById,
            CreatedAt = a.CreatedAt
        });

        return await PaginatedList<AuditLogDto>.CreateAsync(projected, request.PageNumber, request.PageSize, cancellationToken);
    }
}
