using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Reviews.DTOs;

namespace RestaurantSystem.Application.Features.Reviews.Queries.GetMenuItemReviews;

public class GetMenuItemReviewsQueryHandler : IRequestHandler<GetMenuItemReviewsQuery, List<ReviewDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMenuItemReviewsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReviewDto>> Handle(GetMenuItemReviewsQuery request, CancellationToken cancellationToken)
    {
        var orderIds = await _context.OrderItems
            .Where(i => i.ItemId == request.MenuItemId)
            .Select(i => i.OrderId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (orderIds.Count == 0)
        {
            return new List<ReviewDto>();
        }

        return await _context.Reviews
            .AsNoTracking()
            .Where(r => orderIds.Contains(r.OrderId) && r.IsPublished)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                OrderId = r.OrderId,
                CustomerId = r.CustomerId,
                Rating = r.Rating,
                ReviewText = r.ReviewText,
                ResponseText = r.ResponseText,
                CreatedAt = r.CreatedAt,
                RespondedAt = r.RespondedAt
            })
            .ToListAsync(cancellationToken);
    }
}
