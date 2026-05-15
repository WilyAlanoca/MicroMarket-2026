using MicroMarket.Contexto;
using MicroMarket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MicroMarket.Controllers
{
    public class LoginController : Controller
    {
        private readonly MyContext _context;
        private readonly IPasswordHasher<Vendedor> _passwordHasher;
        private readonly IMemoryCache _cache;

        private const int MAX_ATTEMPTS = 5;
        private static readonly TimeSpan LOCKOUT_PERIOD = TimeSpan.FromMinutes(15);

        public LoginController(MyContext context, IPasswordHasher<Vendedor> passwordHasher, IMemoryCache cache)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _cache = cache;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl)
        {
            var attemptsKey = $"login_attempts_{email}";
            var lockoutKey = $"login_lockout_{email}";

            if (_cache.TryGetValue<DateTime>(lockoutKey, out var lockoutEnd) && lockoutEnd > DateTime.UtcNow)
            {
                TempData["LoginError"] = "Cuenta temporalmente bloqueada. Intente más tarde.";
                return RedirectToAction("Index");
            }

            var vendedor = await _context.Vendedores.SingleOrDefaultAsync(v => v.Email == email);

            bool passwordValid = false;
            if (vendedor != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(vendedor, vendedor.PasswordHash, password);
                passwordValid = result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
            }

            if (vendedor != null && passwordValid)
            {
                _cache.Remove(attemptsKey);
                _cache.Remove(lockoutKey);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, vendedor.VendedorId.ToString()),
                    new Claim(ClaimTypes.Name, vendedor.Nombre),
                    new Claim(ClaimTypes.Email, vendedor.Email),
                    new Claim(ClaimTypes.Role, ((int)vendedor.Rol).ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var props = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                int attempts = _cache.Get<int>(attemptsKey);
                attempts++;
                _cache.Set(attemptsKey, attempts, LOCKOUT_PERIOD);

                if (attempts >= MAX_ATTEMPTS)
                {
                    _cache.Set(lockoutKey, DateTime.UtcNow.Add(LOCKOUT_PERIOD), LOCKOUT_PERIOD);
                    _cache.Remove(attemptsKey);
                    TempData["LoginError"] = "Demasiados intentos. Cuenta bloqueada temporalmente.";
                }
                else
                {
                    TempData["LoginError"] = "Credenciales incorrectas.";
                }

                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}

