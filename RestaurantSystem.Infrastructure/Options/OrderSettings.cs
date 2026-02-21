namespace RestaurantSystem.Infrastructure.Options;

public class OrderSettings
{
    public const string SectionName = "Orders";

    public decimal TaxRate { get; set; } = 0.08m;
    public decimal DeliveryFee { get; set; } = 0m;
}
