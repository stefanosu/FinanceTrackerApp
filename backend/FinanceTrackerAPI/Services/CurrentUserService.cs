using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.AspNetCore.Http;

namespace FinanceTrackerAPI.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            _httpContextAccessor = contextAccessor;
        }
        public int? GetUserId()
        {
            // Find the claim and parse it                                                                                                                                                                                                                                                                                                                                                  
            // Step 1: Get the User (ClaimsPrincipal) from HttpContext                                                                                                                                            
            var user = _httpContextAccessor.HttpContext?.User;
            // Step 2: Find the NameIdentifier claim
            var userClaim = user?.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim != null && int.TryParse(userClaim.Value, out int parsedUserId))
            {
                return parsedUserId;
            }
            return null;
        }
    }
}
