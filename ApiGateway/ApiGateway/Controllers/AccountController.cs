using ApiGateway.Entities.Entities;
using ApiGateway.Services.DTOs.Account;
using ApiGateway.Services.DTOs.Mail;
using ApiGateway.Services.Services.Auth;
using ApiGateway.Services.Services.Mail;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace ApiGateway.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private static readonly string[] AllowedRegistrationRoles = { "User", "Staff" };

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailPublisher _emailPublisher;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService tokenService,
            IConfiguration configuration,
            ILogger<AccountController> logger,
            IEmailPublisher emailPublisher)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _configuration = configuration;
            _logger = logger;
            _emailPublisher = emailPublisher;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] Services.DTOs.Account.RegisterRequest request, [FromQuery] string role = "User")
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            role = role.Trim();
            if (!AllowedRegistrationRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Invalid role. Allowed: User, Staff." });
            }

            // Check email existence early
            var existing = await _userManager.FindByEmailAsync(request.Email);
            if (existing != null)
            {
                return Conflict(new { message = "Email already registered." });
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.Phone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest(new
                {
                    message = "User creation failed.",
                    errors = createResult.Errors.Select(e => $"{e.Code}:{e.Description}")
                });
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                // Roll back user if role assignment fails (simplistic compensation)
                await _userManager.DeleteAsync(user);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Role assignment failed." });
            }

            // Preparing confirmation email
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string encodedToken = WebUtility.UrlEncode(token);
            string? callbackUrl = Url.Action(
                nameof(ConfirmEmail),
                "Account",
                new { userId = user.Id, token = encodedToken },
                protocol: Request.Scheme);

            string html = $@"
                    <p>Hi {WebUtility.HtmlEncode(user.UserName)},</p>
                    <p>Thanks for registering at EngConnect. Please confirm your email by clicking the link below:</p>
                    <p><a href=""{callbackUrl}"">Confirm your email</a></p>
                    <p>If you did not create this account, you can ignore this email.</p>";

            // Publish email
            try
            {
                await _emailPublisher.PublishAsync(new EmailEnvelope
                {
                    To = user.Email!,
                    Subject = "Confirm your EngConnect account",
                    Html = html,
                    Template = "ConfirmEmail",
                    CorrelationId = Guid.NewGuid().ToString(),
                    Metadata = new()
                    {
                        ["userId"] = user.Id!,
                        ["callbackUrl"] = callbackUrl ?? string.Empty
                    }
                }, HttpContext.RequestAborted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish email confirmation for user {Email}", user.Email);
            }

            var dto = MapUser(user);
            return StatusCode(StatusCodes.Status201Created, new
            {
                user = dto,
                message = "Registration successful."
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var user = await _userManager.FindByEmailAsync(request.Email!);
            if (user == null)
                return Unauthorized(new { message = "Invalid credentials." });

            if (!await _userManager.CheckPasswordAsync(user, request.Password!))
                return Unauthorized(new { message = "Invalid credentials." });

            if (!user.EmailConfirmed)
            {
                return BadRequest(new { message = "Email not confirmed. Please check your inbox." });
            }

            if (!user.IsActive)
                return Forbid();

            var token = await _tokenService.CreateAsync(user);
            var dto = MapUser(user);

            return Ok(new { token, user = dto });
        }

        [HttpGet("google-signin")]
        [AllowAnonymous]
        public IActionResult GoogleSignIn()
        {
            var redirectUrl = Url.Action(nameof(GoogleResponse), "Account");
            var props = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", props);
        }

        [HttpGet("google-response")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return BadRequest("External login info not found.");

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email)) return BadRequest("Email claim missing.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true, // No need to cf
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    return StatusCode(500, new { message = "Failed to create user from Google." });

                await _userManager.AddToRoleAsync(user, "User");
            }

            // Link login if not already linked
            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            // Ignore failure if already linked (normal duplicate scenario)

            var token = await _tokenService.CreateAsync(user);
            var dto = MapUser(user);

            var allowedFrontendOrigin = _configuration["Auth:Google:FrontendOrigin"]
                ?? "http://localhost:5173";

            // Build the payload separately to avoid brace-escaping issues in the raw string
            var payloadJson = System.Text.Json.JsonSerializer.Serialize(new { token, user = dto });

            var html = $"""
                <html>
                    <body>
                        <script>
                            window.opener && window.opener.postMessage(
                            {payloadJson},
                            "{allowedFrontendOrigin}"
                            );
                            window.close();
                        </script>
                    </body>
                </html>
""";

            return Content(html, "text/html");
        }

        // Protected endpoint (works once FallbackPolicy is enabled)
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            return Ok(MapUser(user));
        }

        private static UserResponse MapUser(ApplicationUser user) =>
            new()
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                IsActive = user.IsActive,
                CreateDate = user.CreatedAt,
                UpdateDate = user.UpdateDate,
                CreateBy = user.CreateBy,
                UpdateBy = user.UpdateBy
            };

        // Callback for email cf
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return BadRequest("Invalid confirmation link.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            string decodedToken = WebUtility.UrlDecode(token);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
                string? redirect = _configuration["SendGrid:ConfirmEmailRedirectUrl"];
                if (!string.IsNullOrWhiteSpace(redirect))
                {
                    return Redirect($"{redirect}?success=true&email={WebUtility.UrlEncode(user.Email)}");
                }

                return Content("<html><body><h3>Email confirmed. You can close this tab and sign in.</h3></body></html>", "text/html");
            }

            return Content("<html><body><h3>Invalid or expired confirmation link.</h3></body></html>", "text/html");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] Services.DTOs.Account.ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(request.Email!);

            // Always return 200 to prevent account enumeration
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return Ok(new { message = "If an account with that email exists, a password reset email has been sent." });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            // Link to GET endpoint that will redirect to FE
            var callbackUrl = Url.Action(
                nameof(BeginResetPassword),
                "Account",
                new { userId = user.Id, token = encodedToken },
                protocol: Request.Scheme
            );

            string html = $@"
        <p>We received a request to reset your EngConnect password.</p>
        <p><a href=""{callbackUrl}"">Reset your password</a></p>
        <p>If you didn't request this, you can safely ignore this email.</p>";


            // Publish
            try
            {
                await _emailPublisher.PublishAsync(new EmailEnvelope
                {
                    To = user.Email!,
                    Subject = "Reset your EngConnect password",
                    Html = html,
                    Template = "PasswordReset",
                    CorrelationId = Guid.NewGuid().ToString(),
                    Metadata = new()
                    {
                        ["userId"] = user.Id!,
                        ["callbackUrl"] = callbackUrl ?? string.Empty
                    }
                }, HttpContext.RequestAborted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish password reset email for user {Email}", user.Email);
            }

            return Ok(new { message = "If an account with that email exists, a password reset email has been sent." });
        }

        // GET: redirect to frontend with the token/userId
        [HttpGet("reset-password")]
        public IActionResult BeginResetPassword([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return BadRequest("Invalid reset link.");

            string? redirect = _configuration["SendGrid:PasswordResetRedirectUrl"];
            if (!string.IsNullOrWhiteSpace(redirect))
            {
                return Redirect($"{redirect}?userId={WebUtility.UrlEncode(userId)}&token={WebUtility.UrlEncode(token)}");
            }

            return Content("<html><body><h3>Open the app to complete your password reset.</h3></body></html>", "text/html");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] Services.DTOs.Account.ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByIdAsync(request.UserId!);
            if (user == null)
                return BadRequest(new { message = "Invalid user." });

            var decodedToken = WebUtility.UrlDecode(request.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken!, request.NewPassword!);

            if (result.Succeeded)
                return Ok(new { message = "Password has been reset successfully." });

            return BadRequest(result.Errors);
        }
    }
}