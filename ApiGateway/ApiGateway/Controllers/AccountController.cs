using ApiGateway.Entities.Entities;
using ApiGateway.Services.DTOs.Account;
using ApiGateway.Services.Services.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
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

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService tokenService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _configuration = configuration;
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
    }
}