using MediatR;
using RestaurantSystem.Application.Features.Reports.DTOs;

namespace RestaurantSystem.Application.Features.Reports.Queries.GetCustomerAnalytics;

public record GetCustomerAnalyticsQuery : IRequest<AnalyticsDto>;
