using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.RestaurantSettings.DTOs;
using RestaurantSystem.Infrastructure.Options;

namespace RestaurantSystem.Infrastructure.Services;

public class OrderSettingsService : IOrderSettings
{
    private readonly OrderSettings _settings;
    private readonly IApplicationDbContext _context;
    private bool _loaded;
    private TaxFeesDto? _taxFees;
    private DeliverySettingsDto? _delivery;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public OrderSettingsService(IOptions<OrderSettings> options, IApplicationDbContext context)
    {
        _settings = options.Value;
        _context = context;
    }

    public decimal TaxRate
    {
        get
        {
            EnsureLoaded();
            return _taxFees?.TaxRate ?? _settings.TaxRate;
        }
    }

    public decimal DeliveryFee
    {
        get
        {
            EnsureLoaded();
            return _delivery?.Fee ?? _settings.DeliveryFee;
        }
    }

    private void EnsureLoaded()
    {
        if (_loaded)
        {
            return;
        }

        _loaded = true;

        var taxSetting = _context.RestaurantSettings
            .AsNoTracking()
            .FirstOrDefault(s => s.Key == "TaxFees");

        if (taxSetting != null)
        {
            _taxFees = TryDeserialize<TaxFeesDto>(taxSetting.Value);
        }

        var deliverySetting = _context.RestaurantSettings
            .AsNoTracking()
            .FirstOrDefault(s => s.Key == "Delivery");

        if (deliverySetting != null)
        {
            _delivery = TryDeserialize<DeliverySettingsDto>(deliverySetting.Value);
        }
    }

    private static T? TryDeserialize<T>(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch
        {
            return default;
        }
    }
}
