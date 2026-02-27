using MediatR;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Application.Features.System.DTOs;

namespace RestaurantSystem.Application.Features.System.Queries.GetAuditLogs;

public class GetAuditLogsQuery : IRequest<PaginatedList<AuditLogDto>>
{
    public string? Table { get; set; }
    public int? UserId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
