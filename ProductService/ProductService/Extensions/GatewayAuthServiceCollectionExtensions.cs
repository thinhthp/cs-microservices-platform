using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace ProductService.Extensions
{
    public sealed class GatewayAuthServiceCollectionExtensions
    : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "Gateway";
        private const string UserIdHeader = "X-User-Id";
        private const string UserRoleHeader = "X-User-Role";
        private const string SignatureHeader = "X-Gateway-Signature";

        private readonly IConfiguration _config;

        public GatewayAuthServiceCollectionExtensions(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration config)
            : base(options, logger, encoder, clock)
        {
            _config = config;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Require the user id header
            if (!Request.Headers.TryGetValue(UserIdHeader, out var userIdValues))
                return Task.FromResult(AuthenticateResult.NoResult());

            string userId = userIdValues.ToString();
            if (string.IsNullOrWhiteSpace(userId))
                return Task.FromResult(AuthenticateResult.Fail("Missing user id."));

            // Role is optional
            Request.Headers.TryGetValue(UserRoleHeader, out var roleValues);
            string? role = roleValues.ToString();
            if (string.IsNullOrWhiteSpace(role))
                role = null;

            // Another layer
            string? sharedSecret = _config["Auth:Gateway:SignatureSecret"];
            if (!string.IsNullOrEmpty(sharedSecret))
            {
                if (!Request.Headers.TryGetValue(SignatureHeader, out var sigValues))
                    return Task.FromResult(AuthenticateResult.Fail("Missing gateway signature."));

                string providedSignature = sigValues.ToString();
                if (!ValidateSignature(sharedSecret, userId, role, providedSignature))
                    return Task.FromResult(AuthenticateResult.Fail("Invalid gateway signature."));
            }

            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, userId)
        };

            if (!string.IsNullOrEmpty(role))
                claims.Add(new Claim(ClaimTypes.Role, role));

            var identity = new ClaimsIdentity(claims, SchemeName, ClaimTypes.Name, ClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }

        private static bool ValidateSignature(string secret, string userId, string? role, string provided)
        {
            // canonical payload
            string payload = $"{userId}.{role ?? string.Empty}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var expected = Convert.ToBase64String(hash);
            // Constant-time compare
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expected),
                Encoding.UTF8.GetBytes(provided));
        }
    }
}