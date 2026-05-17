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
            // 1. Validación básica
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["LoginError"] = "Email y contraseña son requeridos.";
                return RedirectToAction("Index");
            }

            var attemptsKey = $"login_attempts_{email.ToLowerInvariant()}";
            var lockoutKey = $"login_lockout_{email.ToLowerInvariant()}";

            // 2. Verificar bloqueo
            if (_cache.TryGetValue<DateTime>(lockoutKey, out var lockoutEnd) && lockoutEnd > DateTime.UtcNow)
            {
                TempData["LoginError"] = "Cuenta temporalmente bloqueada. Intente más tarde.";
                return RedirectToAction("Index");
            }

            // 3. Buscar vendedor
            var vendedor = await _context.Vendedores
                .FirstOrDefaultAsync(v => v.Email == email);

            bool passwordValid = false;

            if (!string.IsNullOrWhiteSpace(vendedor?.PasswordHash))
            {
                try
                {
                    var result = _passwordHasher.VerifyHashedPassword(vendedor, vendedor.PasswordHash, password);

                    if (result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded)
                    {
                        passwordValid = true;

                        // 4. Rehash si es necesario
                        if (result == PasswordVerificationResult.SuccessRehashNeeded)
                        {
                            vendedor.PasswordHash = _passwordHasher.HashPassword(vendedor, password);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                catch (FormatException)
                {
                    passwordValid = false;
                }
            }

            // 5. Login exitoso
            if (vendedor != null && passwordValid)
            {
                _cache.Remove(attemptsKey);
                _cache.Remove(lockoutKey);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, vendedor.VendedorId.ToString()),
                    new Claim(ClaimTypes.Name, vendedor.Nombre ?? string.Empty),
                    new Claim(ClaimTypes.Email, vendedor.Email ?? string.Empty),

                    // Rol oficial para User.IsInRole(...) y [Authorize(Roles = "...")]
                    new Claim(ClaimTypes.Role, vendedor.Rol.ToString()),

                    // Opcional: rol numérico si tu sistema lo necesita
                    new Claim("RolId", ((int)vendedor.Rol).ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var props = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

                HttpContext.Session.SetInt32("Rol", (int)vendedor.Rol);
                HttpContext.Session.SetInt32("VendedorId", vendedor.VendedorId);
                HttpContext.Session.SetString("Nombre", vendedor.Nombre ?? string.Empty);
                HttpContext.Session.SetString("Email", vendedor.Email ?? string.Empty);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return LocalRedirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                // 6. Incrementar intentos fallidos
                if (_cache.TryGetValue<int>(attemptsKey, out int attempts))
                    attempts++;
                else
                    attempts = 1;

                _cache.Set(attemptsKey, attempts, LOCKOUT_PERIOD);

                if (attempts >= MAX_ATTEMPTS)
                {
                    _cache.Set(lockoutKey, DateTime.UtcNow.Add(LOCKOUT_PERIOD), LOCKOUT_PERIOD);
                    TempData["LoginError"] = "Demasiados intentos. Cuenta bloqueada 15 minutos.";
                }
                else
                {
                    TempData["LoginError"] = $"Credenciales incorrectas. Intentos: {attempts}/{MAX_ATTEMPTS}";
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

