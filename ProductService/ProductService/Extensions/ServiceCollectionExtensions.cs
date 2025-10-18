using Microsoft.AspNetCore.Authentication;

namespace ProductService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGatewayHeaderAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = GatewayAuthServiceCollectionExtensions.SchemeName;
                    options.DefaultChallengeScheme = GatewayAuthServiceCollectionExtensions.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, GatewayAuthServiceCollectionExtensions>(
                    GatewayAuthServiceCollectionExtensions.SchemeName, _ => { });

            services.AddAuthorization(options =>
            {
                // Default policy (role?)
            });

            return services;
        }
    }
}