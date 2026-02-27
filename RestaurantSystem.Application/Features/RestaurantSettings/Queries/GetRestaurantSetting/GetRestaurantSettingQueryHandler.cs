using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;

namespace RestaurantSystem.Application.Features.RestaurantSettings.Queries.GetRestaurantSetting;

public class GetRestaurantSettingQueryHandler : IRequestHandler<GetRestaurantSettingQuery, string?>
{
    private readonly IApplicationDbContext _context;

    public GetRestaurantSettingQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string?> Handle(GetRestaurantSettingQuery request, CancellationToken cancellationToken)
    {
        var setting = await _context.RestaurantSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == request.Key, cancellationToken);

        return setting?.Value;
    }
}
