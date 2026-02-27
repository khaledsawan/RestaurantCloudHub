namespace RestaurantSystem.Application.Features.RestaurantSettings.DTOs;

public class OpeningHoursDto
{
    public List<OpeningDayDto> Days { get; set; } = new();
}

public class OpeningDayDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public bool IsClosed { get; set; }
    public List<TimeRangeDto> Ranges { get; set; } = new();
}

public class TimeRangeDto
{
    public string Open { get; set; } = "09:00";
    public string Close { get; set; } = "17:00";
}
