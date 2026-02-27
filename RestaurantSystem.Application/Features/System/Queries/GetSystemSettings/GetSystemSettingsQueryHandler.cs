using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.System.DTOs;

namespace RestaurantSystem.Application.Features.System.Queries.GetSystemSettings;

public class GetSystemSettingsQueryHandler : IRequestHandler<GetSystemSettingsQuery, List<SystemSettingDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSystemSettingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SystemSettingDto>> Handle(GetSystemSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _context.SystemSettings
            .AsNoTracking()
            .OrderBy(s => s.Key)
            .ToListAsync(cancellationToken);

        return settings.Select(s => new SystemSettingDto
        {
            Key = s.Key,
            Value = JsonDocument.Parse(s.Value).RootElement.Clone(),
            Description = s.Description,
            UpdatedAt = s.UpdatedAt
        }).ToList();
    }
}
