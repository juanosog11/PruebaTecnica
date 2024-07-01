using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Back.Domain.Models;
using Back.Infrastructure.Data;
using Back.Presentacion.Custom;

namespace Back.Presentacion.Controllers
{
    //[EnableCors("AllowAll")]
    [Route("api/[controller]")]

    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly SocialNetworkDbContext _context;
        private readonly ILogger<AccesoController> _logger;
        private readonly Utilidades _utilidades;

        public FollowController(SocialNetworkDbContext context, ILogger<AccesoController> logger, Utilidades utilidades)
        {
            _context = context;
            _logger = logger;
            _utilidades = utilidades;
        }

        [HttpPost]
        public IActionResult FollowUser(int followerId, int followeeId)
        {
            try
            {
                var jwtToken = Request.Cookies["jwt"];
                var principal = _utilidades.GetPrincipalFromToken(jwtToken!);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }

                _logger.LogInformation($"Follower: {followerId}, Followee: {followeeId}");

                // Verificar si ya existe una relación de seguimiento
                var existingFollow = _context.Follows.FirstOrDefault(f => f.FollowerId == followerId && f.FolloweeId == followeeId);

                if (existingFollow != null)
                {
                    // Si la relación ya existe, puedes retornar un mensaje o lo que consideres adecuado
                    return Conflict("Esta relación de seguimiento ya existe.");
                }

                // Si no existe, procede a buscar los usuarios y seguir con la lógica normal
                var follower = _context.Users.Find(followerId);
                var followee = _context.Users.Find(followeeId);

                _logger.LogInformation($"Follower: {follower}, Followee: {followee}");

                if (follower == null || followee == null)
                {
                    return NotFound("Follower or Followee not found.");
                }

                var follow = new Follow
                {
                    FollowerId = followerId,
                    FolloweeId = followeeId
                };

                _context.Follows.Add(follow);
                _context.SaveChanges();

                return Ok("Followed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en GetFollowers: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }


        [HttpDelete]
        public IActionResult UnfollowUser(int followeeId)
        {
            try
            {
                var jwtToken = Request.Cookies["jwt"];
                var principal = _utilidades.GetPrincipalFromToken(jwtToken);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }

                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "usuarioId");
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int followerId))
                {
                    return Unauthorized(new { isSuccess = false, message = "No se pudo obtener el usuario actual." });
                }

                var follow = _context.Follows.FirstOrDefault(f => f.FollowerId == followerId && f.FolloweeId == followeeId);

                if (follow == null)
                {
                    return NotFound("La relación de seguimiento no se encontró.");
                }

                _context.Follows.Remove(follow);
                _context.SaveChanges();

                return Ok("Dejaste de seguir exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al dejar de seguir usuario: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpGet("followers/{followeeId}")]
        public IActionResult GetFollowers(int followeeId)
        {
            try
            {
                var jwtToken = Request.Cookies["jwt"];
                var principal = _utilidades.GetPrincipalFromToken(jwtToken);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }

                _logger.LogInformation($"Getting followers for user with ID: {followeeId}");

                var followers = _context.Follows
                    .Where(f => f.FolloweeId == followeeId)
                    .Include(f => f.Follower)
                    .Select(f => f.Follower)
                    .ToList();

                return Ok(followers);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en GetFollowers: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpGet("following/{followerId}")]
        public IActionResult GetFollowing(int followerId)
        {
            try
            {
                var jwtToken = Request.Cookies["jwt"];
                var principal = _utilidades.GetPrincipalFromToken(jwtToken);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }

                _logger.LogInformation($"Getting users followed by user with ID: {followerId}");

                var following = _context.Follows
                    .Where(f => f.FollowerId == followerId)
                    .Include(f => f.Followee)
                    .Select(f => f.Followee)
                    .ToList();

                return Ok(following);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en GetFollowing: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }


        [HttpGet("GetNoFollower/{followerId}")]
        public async Task<IActionResult> GetNoFollower(int followerId)
        {
            try
            {
                var jwtToken = Request.Cookies["jwt"];
                var principal = _utilidades.GetPrincipalFromToken(jwtToken);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }

                // Obtener los IDs de los usuarios que sigue el usuario con followerId
                var followeeIds = _context.Follows
                    .Where(f => f.FollowerId == followerId)
                    .Select(f => f.FolloweeId)
                    .ToList();

                // Obtener todos los usuarios excluyendo a los que sigue el usuario con followerId
                var usuarios = await _context.Users
                    .Where(u => u.UsuarioId != followerId && !followeeIds.Contains(u.UsuarioId))
                    .Select(u => new { u.UsuarioId, u.Usuario })
                    .ToListAsync();

                _logger.LogInformation($"Successfully retrieved users not followed by user with ID: {followerId}");

                _logger.LogInformation($"Successfully retrieved users not followed by user with ID: {usuarios}");

                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en GetNoFollower: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpGet("checkIfFollowing/{followerId}/{followeeId}")]
        public IActionResult CheckIfFollowing(int followerId, int followeeId)
        {
            try
            {
                var jwtToken = Request.Cookies["jwt"];
                var principal = _utilidades.GetPrincipalFromToken(jwtToken);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }

                _logger.LogInformation($"Checking if user with ID {followerId} follows user with ID {followeeId}");

                // Verificar si existe una relación de seguimiento
                var existingFollow = _context.Follows.FirstOrDefault(f => f.FollowerId == followerId && f.FolloweeId == followeeId);

                if (existingFollow != null)
                {
                    return Ok(true); // Devolver true si existe la relación de seguimiento
                }
                else
                {
                    return Ok(false); // Devolver false si no existe la relación de seguimiento
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en CheckIfFollowing: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

    }
}
