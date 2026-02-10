using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceTrackerAPI.Services.Interfaces
{
    public interface ICurrentUserService
    {
        int? GetUserId();
    }
}
