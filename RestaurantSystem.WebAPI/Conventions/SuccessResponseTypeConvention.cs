using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace RestaurantSystem.WebAPI.Conventions;

public sealed class SuccessResponseTypeConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            foreach (var action in controller.Actions)
            {
                var responseType = ResolveResponseType(action);
                if (responseType == null)
                {
                    continue;
                }

                var hasExplicitSuccess = action.Filters.OfType<ProducesResponseTypeAttribute>()
                    .Any(a => a.StatusCode is >= 200 and < 300);

                if (hasExplicitSuccess)
                {
                    continue;
                }

                action.Filters.Add(new ProducesResponseTypeAttribute(responseType, StatusCodes.Status200OK));
            }
        }
    }

    private static Type? ResolveResponseType(ActionModel action)
    {
        var returnType = action.ActionMethod.ReturnType;

        if (typeof(Task).IsAssignableFrom(returnType) && returnType.IsGenericType)
        {
            returnType = returnType.GetGenericArguments()[0];
        }

        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ActionResult<>))
        {
            return returnType.GetGenericArguments()[0];
        }

        if (typeof(IActionResult).IsAssignableFrom(returnType))
        {
            return null;
        }

        return returnType;
    }
}
