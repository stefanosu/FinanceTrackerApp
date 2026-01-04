using System;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
            var methodName = actionMethod.ActionName;
            if (methodName.Contains("getbyid", StringComparison.OrdinalIgnoreCase) ||
                methodName.Contains("update", StringComparison.OrdinalIgnoreCase) ||
                methodName.Contains("delete", StringComparison.OrdinalIgnoreCase))
            {
                // Look for 'id' or 'userId' parameter
                string[] idParameterNames = { "id", "userId" };
                foreach (var paramName in idParameterNames)
                {
                    if (context.ActionArguments.TryGetValue(paramName, out var idValue) && idValue is int id)
                    {
                        if (id <= 0)
                        {
                            context.Result = new BadRequestObjectResult($"Invalid ID: {id}. ID must be a positive integer.");
                            return;
                        }
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
