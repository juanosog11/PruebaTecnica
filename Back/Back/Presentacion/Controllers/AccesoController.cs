using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Cors;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Back.Domain.Models;
using Back.Infrastructure.Data;
using Back.Presentacion.Custom;


namespace Back.Presentacion.Controllers
{
    //[EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccesoController : ControllerBase
    {
        private readonly SocialNetworkDbContext _context;
        private readonly Utilidades _utilidades;
        private readonly ILogger<AccesoController> _logger;
        private readonly IConfiguration _configuration;

        public AccesoController(SocialNetworkDbContext context, Utilidades utilidades, ILogger<AccesoController> logger, IConfiguration configuration)
        {
            _context = context;
            _utilidades = utilidades;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Registrarse")]
        public async Task<IActionResult> Registrarse([FromBody] User objeto)
        {
            try
            {
                // Verificar si el nombre de usuario, el correo electrónico o el número ya existen en la base de datos
                bool usuarioExistente = await _context.Users.AnyAsync(u => u.Usuario == objeto.Usuario);
                bool emailExistente = await _context.Users.AnyAsync(u => u.Email == objeto.Email);
                bool numeroExistente = await _context.Users.AnyAsync(u => u.Numero == objeto.Numero);

                // Si alguno de los valores ya existe, devolver un mensaje de error adecuado
                if (usuarioExistente)
                {
                    return BadRequest(new { isSuccess = false, message = "El nombre de usuario ya existe." });
                }
                if (emailExistente)
                {
                    return BadRequest(new { isSuccess = false, message = "El correo electrónico ya está registrado." });
                }
                if (numeroExistente)
                {
                    return BadRequest(new { isSuccess = false, message = "El número ya está registrado." });
                }

                // Crear el modelo de usuario con los datos proporcionados
                var modeloUsuario = new User
                {
                    Usuario = objeto.Usuario,
                    Email = objeto.Email,
                    Numero = objeto.Numero,
                    Clave = _utilidades.encriptarSha256(objeto.Clave)
                };

                // Guardar el nuevo usuario en la base de datos
                await _context.AddAsync(modeloUsuario);
                await _context.SaveChangesAsync();

                // Verificar si el usuario se guardó correctamente y devolver el token de JWT
                if (modeloUsuario.UsuarioId != 0)
                {
                    // Generar token JWT
                    var token = _utilidades.generarJWT(modeloUsuario);

                    // Configurar la cookie para el token JWT
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = Request.IsHttps,
                        SameSite = SameSiteMode.None, // Importante para CORS con credenciales
                        Expires = DateTime.UtcNow.AddHours(1) // Duración de la cookie
                    };

                    Response.Cookies.Append("jwt", token, cookieOptions);


                    return StatusCode(StatusCodes.Status201Created, new
                    {
                        isSuccess = true,
                        token, // Devolver el token en la respuesta si es necesario
                        IdUsuario = modeloUsuario.UsuarioId,
                        Usuario = modeloUsuario
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar registrar el usuario");
                return BadRequest(new { isSuccess = false, message = "Error al registrar, inténtelo más tarde" });
            }
        }

        [HttpPost]
        [Route("Ingresar")]
        public async Task<IActionResult> Login([FromBody] Login objeto)
        {
            try
            {
                _logger.LogInformation("Intento de inicio de sesión con el email: {Email}", objeto.Email);

                var usuarioEncontrado = await _context.Users
                    .Where(u => u.Email == objeto.Email && u.Clave == _utilidades.encriptarSha256(objeto.Clave))
                    .FirstOrDefaultAsync();

                if (usuarioEncontrado == null)
                {
                    _logger.LogWarning("Usuario no encontrado para el email: {Email}", objeto.Email);
                    return BadRequest(new { isSuccess = false, message = "Usuario no encontrado. Por favor verifique sus credenciales." });
                }
                else
                {
                    _logger.LogInformation("Usuario encontrado para el email: {Email}", objeto.Email);

                    // Generar token JWT
                    var token = _utilidades.generarJWT(usuarioEncontrado);


                    // Configurar la cookie para el token JWT
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = Request.IsHttps,
                        SameSite = SameSiteMode.None, // Importante para CORS con credenciales
                        Expires = DateTime.UtcNow.AddHours(1) // Duración de la cookie
                    };

                    Response.Cookies.Append("jwt", token, cookieOptions);




                    return StatusCode(StatusCodes.Status200OK, new
                    {
                        isSuccess = true,
                        token, // Devolver el token en la respuesta si es necesario
                        IdUsuario = usuarioEncontrado.UsuarioId,
                        Usuario = usuarioEncontrado
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar iniciar sesión");
                return BadRequest(new { isSuccess = false, message = "Error interno del servidor. Error: " + ex.ToString() });
            }
        }

        [HttpPost]
        [Route("CerrarSesion")]
        public IActionResult CerrarSesion()
        {
            try
            {
                // Eliminar la cookie de autenticación
                Response.Cookies.Delete("jwt", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Domain = "localhost"
                });

                return Ok(new { isSuccess = true, message = "Sesión cerrada correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar cerrar sesión");
                return BadRequest(new { isSuccess = false, message = "Error interno del servidor. Error: " + ex.ToString() });
            }
        }


        [HttpGet]
        [Route("VerificarToken")]
        public async Task<IActionResult> VerificarToken()
        {
            try
            {
                // Obtener el token JWT desde las cookies
                var jwtToken = Request.Cookies["jwt"];

                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Ok(new { isSuccess = false, message = "No se encontró el token JWT en las cookies." });
                }

                // Verificar y decodificar el token JWT
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

                SecurityToken validatedToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out validatedToken);

                // Obtener la identidad del usuario desde el token
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value;
                var userPhone = principal.FindFirst(ClaimTypes.MobilePhone)?.Value;

                try
                {
                    int userIdInt = int.Parse(userId!);

                    var usuarioEncontrado = await _context.Users
                        .Where(u => u.UsuarioId == userIdInt)
                        .FirstOrDefaultAsync();

                    // Devolver la información del usuario si es necesario
                    return Ok(new
                    {
                        isSuccess = true,
                        usuario = usuarioEncontrado
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al buscar el usuario");
                    return BadRequest(new { isSuccess = false, message = "Error al buscar el usuario" });
                }
            }
            catch (SecurityTokenExpiredException)
            {
                // Devolver un estado Unauthorized si el token ha expirado
                return StatusCode(StatusCodes.Status401Unauthorized, new { isSuccess = false, message = "El token JWT ha expirado." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar el token JWT.");
                return BadRequest(new { isSuccess = false, message = "Error al verificar el token JWT." });
            }
        }



    }
}
