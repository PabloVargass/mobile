using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CleanOrderAPI.Data.Entities;
using Microsoft.Extensions.Configuration;

namespace CleanOrderAPI.Services
{
    public class JWTService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public JWTService(IConfiguration configuration)
        {
            _secretKey = configuration["JwtSettings:SecretKey"]!;
            _issuer = configuration["JwtSettings:Issuer"]!;
            _audience = configuration["JwtSettings:Audience"]!;
        }

        public string GenerateJwtToken(Usuario user)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Use numeric role id to avoid null navigation issues
            string roleId = user.FkIdRol.ToString();

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Correo),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, roleId) // role as string id (e.g., "1")
            };

            // Optional: include a human-readable role name if available
            if (user.FkIdRolNavigation?.Nombre is string roleName && !string.IsNullOrWhiteSpace(roleName))
            {
                claims.Add(new Claim("roleName", roleName));
            }

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
