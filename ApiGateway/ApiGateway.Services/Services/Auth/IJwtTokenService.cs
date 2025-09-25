using ApiGateway.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Services.Services.Auth
{
    public interface IJwtTokenService
    {
        Task<string> CreateAsync(ApplicationUser user, CancellationToken ct = default);
    }
}
