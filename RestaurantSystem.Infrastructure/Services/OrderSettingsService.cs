using Microsoft.Extensions.Options;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Infrastructure.Options;

namespace RestaurantSystem.Infrastructure.Services;

public class OrderSettingsService : IOrderSettings
{
    private readonly OrderSettings _settings;

    public OrderSettingsService(IOptions<OrderSettings> options)
    {
        _settings = options.Value;
    }

    public decimal TaxRate => _settings.TaxRate;
    public decimal DeliveryFee => _settings.DeliveryFee;
}
