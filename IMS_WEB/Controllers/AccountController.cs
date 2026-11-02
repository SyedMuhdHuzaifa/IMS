using System.Security.Claims;
using DTOs.Auth;
using IMS_WEB.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS_WEB.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthApiServices _authApiService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthApiServices authApiService, ILogger<AccountController> logger)
        {
            _authApiService = authApiService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Product");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginRequest());
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest request, string? returnUrl = null, CancellationToken ct = default)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var authResponse = await _authApiService.LoginAsync(request, ct);

            if (authResponse == null || string.IsNullOrEmpty(authResponse.Token))
            {
                ModelState.AddModelError(string.Empty, "Invalid username/email or password.");
                return View(request);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, authResponse.UserId),
                new Claim(ClaimTypes.Name, authResponse.UserName),
                new Claim(ClaimTypes.Email, authResponse.Email)
            };

            foreach (var role in authResponse.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = authResponse.ExpiresAt
            };

            authProperties.StoreTokens(new[]
            {
                new AuthenticationToken { Name = "access_token", Value = authResponse.Token }
            });

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("User {UserName} logged in successfully.", authResponse.UserName);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Product");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}