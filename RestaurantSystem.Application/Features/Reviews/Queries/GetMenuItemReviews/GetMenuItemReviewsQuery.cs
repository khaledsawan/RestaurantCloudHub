using MediatR;
using RestaurantSystem.Application.Features.Reviews.DTOs;

namespace RestaurantSystem.Application.Features.Reviews.Queries.GetMenuItemReviews;

public record GetMenuItemReviewsQuery(int MenuItemId) : IRequest<List<ReviewDto>>;
