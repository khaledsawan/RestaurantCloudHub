namespace RestaurantSystem.Application.Common.Interfaces;

public interface IOrderSettings
{
    decimal TaxRate { get; }
    decimal DeliveryFee { get; }
}
