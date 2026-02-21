namespace RestaurantSystem.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Preparing = 2,
    Ready = 3,
    OutForDelivery = 4,
    Completed = 5,
    Cancelled = 6
}
