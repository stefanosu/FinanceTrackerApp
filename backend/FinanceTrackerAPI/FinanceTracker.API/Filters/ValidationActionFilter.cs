using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace FinanceTrackerAPI.FinanceTracker.API.Filters
{
    public class ValidationActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Get the action method info
            var actionMethod = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
            if (actionMethod == null) return;

            // Check if this is a method that should validate IDs
            var methodName = actionMethod.ActionName.ToLower();
            if (methodName.Contains("getbyid") || methodName.Contains("update") || methodName.Contains("delete"))
            {
                // Look for 'id' parameter specifically
                if (context.ActionArguments.TryGetValue("id", out var idValue) && idValue is int id)
                {
                    if (id <= 0)
                    {
                        context.Result = new BadRequestObjectResult($"Invalid ID: {id}. ID must be a positive integer.");
                        return;
                    }
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // This method is called after the action executes
            // We could add post-processing logic here if needed
        }
    }
}
