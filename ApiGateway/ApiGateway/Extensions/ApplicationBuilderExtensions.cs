using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ApiGateway.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task SeedRolesAsync(this IApplicationBuilder app)
        {
            // Create a scope to retrieve scoped services
            using (IServiceScope scope = app.ApplicationServices.CreateScope())
            {
                // Retrieve the RoleManager from DI
                RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Define the roles to seed
                string[] roleNames = { "Admin", "User", "Staff" };

                // Loop through each role and create it if it doesn't exist
                foreach (string roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
            }
        }

        // Reverse proxy pipeline (currently applied for all)
        public static IEndpointConventionBuilder MapGatewayReverseProxy(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapReverseProxy(proxyPipeline =>
            {
                proxyPipeline.Use(async (context, next) =>
                {
                    // Inject user context for downstream services
                    if (context.User.Identity?.IsAuthenticated == true)
                    {
                        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;
                        // Another layer for downstream
                        var secret = context.RequestServices.GetRequiredService<IConfiguration>()["Auth:Gateway:SignatureSecret"];
                        if (!string.IsNullOrEmpty(secret) && !string.IsNullOrEmpty(userId))
                        {
                            var payload = $"{userId}.{userRole ?? string.Empty}";
                            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
                            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
                            context.Request.Headers["X-Gateway-Signature"] = signature;
                        }

                        if (!string.IsNullOrEmpty(userId))
                            context.Request.Headers["X-User-Id"] = userId;

                        if (!string.IsNullOrEmpty(userRole))
                            context.Request.Headers["X-User-Role"] = userRole;
                    }

                    await next();
                });
            });
        }
    }
}