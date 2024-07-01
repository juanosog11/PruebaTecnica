using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Back.Domain.Models;

namespace Back.Presentacion.Custom
{
    public class Utilidades
    {
        private readonly IConfiguration _configuration;

        public Utilidades(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string encriptarSha256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // COMPUTE EL HASH
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convertir el array de bytes a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public string generarJWT(User modelo)
        {
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, modelo.UsuarioId.ToString()),
                new Claim(ClaimTypes.Email, modelo.Email),
                new Claim(ClaimTypes.MobilePhone, modelo.Numero.ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var jwtConfig = new JwtSecurityToken(
                claims: userClaims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }

        public ClaimsPrincipal GetPrincipalFromToken(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Opcional: reducir la tolerancia para la expiración
            };

            try
            {
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
