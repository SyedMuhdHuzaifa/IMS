using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DTOs.Auth;
using IMS.Data;
using IMS.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IMS_API.Controllers
{
    [Route("api/auth")]
    public class AuthController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            AppDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserNameOrEmail)
                ?? await _userManager.FindByEmailAsync(request.UserNameOrEmail);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Unauthorized(new { message = "Invalid username/email or password." });
            }

            if (!user.IsActive)
            {
                return Unauthorized(new { message = "This account has been deactivated. Please contact an administrator." });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles, out var expiresAt, out var jwtId);
            var refreshToken = await GenerateAndSaveRefreshTokenAsync(user.Id, jwtId);

            return Ok(new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken.Token,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList()
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _userManager.FindByNameAsync(request.UserName) != null)
            {
                return BadRequest(new { message = $"Username '{request.UserName}' is already taken." });
            }

            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return BadRequest(new { message = $"Email '{request.Email}' is already registered." });
            }

            var user = new ApplicationUser
            {
                UserName = request.UserName.Trim(),
                Email = request.Email.Trim(),
                FullName = request.FullName?.Trim() ?? request.UserName.Trim(),
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Registration failed", errors = result.Errors.Select(e => e.Description) });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles, out var expiresAt, out var jwtId);
            var refreshToken = await GenerateAndSaveRefreshTokenAsync(user.Id, jwtId);

            return Ok(new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken.Token,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList()
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == request.RefreshToken);

            if (storedToken == null)
            {
                return Unauthorized(new { message = "Invalid refresh token." });
            }

            if (!storedToken.IsActive)
            {
                return Unauthorized(new { message = "Refresh token is expired or has been revoked." });
            }

            var user = await _userManager.FindByIdAsync(storedToken.UserId);
            if (user == null || !user.IsActive)
            {
                return Unauthorized(new { message = "User not found or deactivated." });
            }

            // Mark old refresh token as revoked/used
            storedToken.RevokedAt = DateTime.UtcNow;
            _context.RefreshTokens.Update(storedToken);

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles, out var expiresAt, out var jwtId);
            var newRefreshToken = await GenerateAndSaveRefreshTokenAsync(user.Id, jwtId);

            await _context.SaveChangesAsync();

            return Ok(new AuthResponse
            {
                Token = token,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList()
            });
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FullName,
                user.IsActive,
                Roles = roles
            });
        }

        private string GenerateJwtToken(ApplicationUser user, IList<string> roles, out DateTime expiresAt, out string jwtId)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secret = jwtSettings["Secret"] ?? "A_VERY_SECURE_AND_LONG_KEY_FOR_IMS_ENTERPRISE_APPLICATION_2026_SECRET_KEY!";
            var issuer = jwtSettings["Issuer"] ?? "IMS_API";
            var audience = jwtSettings["Audience"] ?? "IMS_Client";
            var expiryMinutes = int.TryParse(jwtSettings["ExpiryMinutes"], out var mins) ? mins : 120;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            jwtId = Guid.NewGuid().ToString();
            expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Jti, jwtId),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? string.Empty)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<RefreshToken> GenerateAndSaveRefreshTokenAsync(string userId, string jwtId)
        {
            var expiryDays = int.TryParse(_configuration["JwtSettings:RefreshTokenExpiryDays"], out var days) ? days : 7;

            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
                RevokedAt = null
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }
    }
}
