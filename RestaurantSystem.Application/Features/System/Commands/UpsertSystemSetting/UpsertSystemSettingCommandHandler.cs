using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Features.System.Commands.UpsertSystemSetting;

public class UpsertSystemSettingCommandHandler : IRequestHandler<UpsertSystemSettingCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpsertSystemSettingCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpsertSystemSettingCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Key))
        {
            return Result.Failure("Key is required");
        }

        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == request.Key, cancellationToken);

        if (setting == null)
        {
            setting = new SystemSetting
            {
                Key = request.Key,
                Value = request.Value,
                Description = request.Description,
                UpdatedById = _currentUserService.UserId
            };
            _context.SystemSettings.Add(setting);
        }
        else
        {
            setting.Value = request.Value;
            setting.Description = request.Description;
            setting.UpdatedById = _currentUserService.UserId;
            setting.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
