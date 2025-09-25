using ApiGateway.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Services.Services.UserContext
{
    public interface IUserContextService
    {
        bool IsAuthenticated { get; }
        string? GetCurrentUserId();
        Task<ApplicationUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default);
        Task<IList<string>> GetCurrentUserRolesAsync(CancellationToken cancellationToken = default);
    }
}
