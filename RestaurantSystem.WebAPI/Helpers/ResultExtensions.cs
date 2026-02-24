using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RestaurantSystem.WebAPI.Helpers;

public static class ResultExtensions
{
    public static ActionResult ToValidationProblem(this ControllerBase controller, IEnumerable<string> errors)
    {
        var modelState = new ModelStateDictionary();
        foreach (var error in errors)
        {
            modelState.AddModelError("errors", error);
        }

        return controller.ValidationProblem(modelState);
    }

    public static ActionResult ToValidationProblem(this ControllerBase controller, string error)
    {
        return controller.ToValidationProblem(new[] { error });
    }
}
