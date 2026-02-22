namespace RestaurantSystem.Application.Features.Reports.DTOs;

public class AnalyticsDto
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
}
