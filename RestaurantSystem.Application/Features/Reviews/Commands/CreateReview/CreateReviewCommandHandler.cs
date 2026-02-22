using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Reviews.Commands.CreateReview;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateReviewCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return Result.Failure("User not authenticated");
        }

        var customerId = await _context.Customers
            .Where(c => c.UserId == _currentUserService.UserId.Value)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!customerId.HasValue)
        {
            return Result.Failure("Customer profile not found");
        }

        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.CustomerId == customerId.Value, cancellationToken);

        if (order == null)
        {
            return Result.Failure("Order not found");
        }

        if (order.OrderStatus != OrderStatus.Completed)
        {
            return Result.Failure("Reviews can only be submitted for completed orders");
        }

        var alreadyReviewed = await _context.Reviews
            .AnyAsync(r => r.OrderId == order.Id && r.CustomerId == customerId.Value, cancellationToken);

        if (alreadyReviewed)
        {
            return Result.Failure("Review already submitted for this order");
        }

        _context.Reviews.Add(new Review
        {
            OrderId = order.Id,
            CustomerId = customerId.Value,
            Rating = request.Rating,
            FoodRating = request.FoodRating,
            ServiceRating = request.ServiceRating,
            DeliveryRating = request.DeliveryRating,
            ReviewText = request.ReviewText,
            IsPublished = true
        });

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
