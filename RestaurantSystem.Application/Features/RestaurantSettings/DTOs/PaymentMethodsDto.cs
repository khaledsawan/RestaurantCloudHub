using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.RestaurantSettings.DTOs;

public class PaymentMethodsDto
{
    public List<PaymentMethod> EnabledMethods { get; set; } = new();
}
