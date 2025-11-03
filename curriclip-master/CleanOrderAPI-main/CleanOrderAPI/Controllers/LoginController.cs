using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using CleanOrderAPI.Data;
using CleanOrderAPI.Data.Entities;
using CleanOrderAPI.Services;

namespace CleanOrderAPI.Controllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JWTService _jwtService;
        private readonly PasswordService _passwordService;
        private const string JwtCookieName = "AuthToken";

        public LoginController(ApplicationDbContext context, JWTService jwtService, PasswordService passwordService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordService = passwordService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.correo) || string.IsNullOrWhiteSpace(request.password))
            {
                return BadRequest("Correo y password son obligatorios.");
            }

            // Find user by email only
            Usuario? user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == request.correo);

            if (user is null)
            {
                return Unauthorized("Correo o password invalidos.");
            }

            if (user.Activo != 1)
            {
                return StatusCode(403, "Usuario inactivo. Contacte al administrador.");
            }

            // Verify password using BCrypt
            bool isPasswordValid = _passwordService.VerifyPassword(request.password, user.Password);

            if (!isPasswordValid)
            {
                return Unauthorized("Correo o password invalidos.");
            }

            // Check if user is active
            if (user.Activo != 1)
            {
                return Unauthorized("Usuario inactivo.");
            }

            // Generate JWT Token
            string token = _jwtService.GenerateJwtToken(user);

            // Set HttpOnly cookie for Angular (requires HTTPS + SameSite=None)
            CookieOptions cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,                 // requires HTTPS on both API and Angular
                SameSite = SameSiteMode.None,  // allow cross-site from Angular origin
                Expires = DateTimeOffset.UtcNow.AddHours(1),
                Path = "/",
                IsEssential = true
            };

            Response.Cookies.Append(JwtCookieName, token, cookieOptions);

            // Optionally return some basic info (not the token)
            return Ok(new
            {
                correo = user.Correo,
                role = user.FkIdRol
            });
        }

        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            // Delete the auth cookie
            Response.Cookies.Delete(JwtCookieName, new CookieOptions
            {
                Path = "/",
                SameSite = SameSiteMode.None,
                Secure = true,
                HttpOnly = true
            });

            return NoContent();
        }

        // 200 if authenticated (JWT cookie valid), 401 otherwise
        [HttpGet]
        [Route("me")]
        [Authorize]
        public IActionResult Session()
        {
            ClaimsPrincipal user = HttpContext.User;
            return Ok(new
            {
                authenticated = true,
                correo = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value,
                role = user.FindFirst(ClaimTypes.Role)?.Value
            });
        }
    }

    public class LoginRequest
    {
        public required string correo { get; set; }
        public required string password { get; set; }
    }
}