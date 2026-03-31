using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Infrastructure.RateLimiting;
using RestaurantSystem.Application.Common.Exceptions;
using RestaurantSystem.Infrastructure.Hubs;

namespace RestaurantSystem.WebAPI.Extensions;

public static class WebApplicationExtensions
{
        public static WebApplication UseWebApiPipeline(this WebApplication app)
        {
            var enableSwaggerInProduction = app.Configuration.GetValue<bool>("Swagger:EnableInProduction");
            if (app.Environment.IsDevelopment() || enableSwaggerInProduction)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "API v2");
                    c.SwaggerEndpoint("/swagger/health/swagger.json", "Health Checks");
                });
            }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var problemDetailsService = context.RequestServices.GetRequiredService<IProblemDetailsService>();
                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (exceptionHandlerFeature?.Error is ValidationException validationException)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await problemDetailsService.WriteAsync(new ProblemDetailsContext
                        {
                            HttpContext = context,
                            ProblemDetails = new ValidationProblemDetails(validationException.Errors)
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Title = "One or more validation failures have occurred."
                            }
                        });
                        return;
                    }

                    if (exceptionHandlerFeature?.Error != null)
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        await problemDetailsService.WriteAsync(new ProblemDetailsContext
                        {
                            HttpContext = context,
                            ProblemDetails = new ProblemDetails
                            {
                                Status = StatusCodes.Status500InternalServerError,
                                Title = "An unexpected error occurred."
                            }
                        });
                    }
                });
            });
            app.UseHsts();
        }

        app.Use(async (context, next) =>
        {
            context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
            context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
            context.Response.Headers.TryAdd("Referrer-Policy", "no-referrer");
            context.Response.Headers.TryAdd("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
            await next();
        });

        app.UseForwardedHeaders();
        app.UseStaticFiles();
        app.UseHttpsRedirection();
        app.UseCors("AllowFrontend");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseApiRateLimiting();
        app.UseStatusCodePages(async context =>
        {
            var httpContext = context.HttpContext;
            if (httpContext.Response.HasStarted)
            {
                return;
            }

            var problemDetailsService = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = new ProblemDetails
                {
                    Status = httpContext.Response.StatusCode,
                    Title = ReasonPhrases.GetReasonPhrase(httpContext.Response.StatusCode)
                }
            });
        });
        app.MapControllers();
        app.MapHub<OrderHub>("/hubs/orders").RequireAuthorization();
        app.MapHub<KitchenHub>("/hubs/kitchen").RequireAuthorization();
        app.MapHub<NotificationHub>("/hubs/notifications").RequireAuthorization();
        app.MapHub<DeliveryHub>("/hubs/delivery").RequireAuthorization();
        app.MapHub<DashboardHub>("/hubs/dashboard").RequireAuthorization();

        return app;
    }
}
