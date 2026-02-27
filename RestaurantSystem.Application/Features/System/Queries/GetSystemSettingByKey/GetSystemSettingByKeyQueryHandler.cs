using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.System.DTOs;

namespace RestaurantSystem.Application.Features.System.Queries.GetSystemSettingByKey;

public class GetSystemSettingByKeyQueryHandler : IRequestHandler<GetSystemSettingByKeyQuery, SystemSettingDto?>
{
    private readonly IApplicationDbContext _context;

    public GetSystemSettingByKeyQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SystemSettingDto?> Handle(GetSystemSettingByKeyQuery request, CancellationToken cancellationToken)
    {
        var setting = await _context.SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == request.Key, cancellationToken);

        if (setting == null)
        {
            return null;
        }

        return new SystemSettingDto
        {
            Key = setting.Key,
            Value = JsonDocument.Parse(setting.Value).RootElement.Clone(),
            Description = setting.Description,
            UpdatedAt = setting.UpdatedAt
        };
    }
}
