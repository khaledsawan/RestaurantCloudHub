using MediatR;
using RestaurantSystem.Application.Features.Reports.DTOs;

namespace RestaurantSystem.Application.Features.Reports.Queries.GetPopularItems;

public record GetPopularItemsQuery : IRequest<List<PopularItemDto>>
{
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
    public int Limit { get; init; } = 10;
}
