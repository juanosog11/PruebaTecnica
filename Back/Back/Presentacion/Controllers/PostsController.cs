using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Back.Domain.Models;
using Back.Infrastructure.Data;
using Back.Presentacion.Custom;

namespace Back.Presentacion.Controllers
{
    //[EnableCors("AllowAll")]
    [Route("api/[controller]")]

    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly SocialNetworkDbContext _context;
        private readonly Utilidades _utilidades;
        private readonly ILogger<AccesoController> _logger;

        public PostsController(SocialNetworkDbContext context, Utilidades utilidades, ILogger<AccesoController> logger)
        {
            _context = context;
            _utilidades = utilidades;
            _logger = logger;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            try
            {
                var jwtToken = Request.Cookies["jwt"];
                var principal = _utilidades.GetPrincipalFromToken(jwtToken!);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }

                return Ok(await _context.Posts.ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Posts/5
        [HttpGet("{id}", Name = "GetPost")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            try
            {
                _logger.LogError("id de busqueda " + id);
                var jwtToken = Request.Cookies["jwt"];
                var principal = _utilidades.GetPrincipalFromToken(jwtToken!);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }

                var post = await _context.Posts.FirstOrDefaultAsync(g => g.PostId == id);

                if (post == null)
                {
                    return NotFound(new { isSuccess = false, message = $"No se encontró ningún post con ID {id}." });
                }

                return Ok(post);
            }
            catch (Exception ex)
            {
                return BadRequest(new { isSuccess = false, message = ex.Message });
            }
        }


        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, [FromBody] Post post)
        {
            try
            {
                var jwtToken = Request.Cookies["jwt"];
                var principal = _utilidades.GetPrincipalFromToken(jwtToken!);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }

                if (post.PostId == id)
                {
                    _context.Entry(post).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return CreatedAtRoute("GetPost", new { id = post.UsuarioId }, post);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> PostPost([FromBody] Post post)
        {
            try
            {
                var jwtToken = Request.Cookies["jwt"];
                var principal = _utilidades.GetPrincipalFromToken(jwtToken!);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }

                // Simplemente agregar el post sin hacer más validaciones
                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetPost", new { id = post.PostId }, post);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post");
                return BadRequest(ex.Message);
            }
        }









        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                var jwtToken = Request.Cookies["jwt"];
                var principal = _utilidades.GetPrincipalFromToken(jwtToken!);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }

                var post = await _context.Posts.FindAsync(id);
                if (post == null)
                {
                    return NotFound();
                }



                try
                {
                    _context.Posts.Remove(post);
                    await _context.SaveChangesAsync();
                    return Ok(id);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Manejar errores de concurrencia optimista
                    // Recargar la entidad y verificar cambios antes de eliminar
                    var entry = _context.Entry(post);
                    if (entry.State == EntityState.Deleted)
                    {
                        // La entidad fue eliminada por otro proceso, manejar según necesidades
                        return Conflict(new { isSuccess = false, message = "No se pudo eliminar el post debido a un conflicto de concurrencia optimista. " + ex });
                    }
                    else
                    {
                        // Manejar otros errores de concurrencia optimista según tus necesidades
                        // Por ejemplo, puedes intentar eliminar nuevamente o regresar un error específico
                        return BadRequest(new { isSuccess = false, message = "Error de concurrencia optimista al intentar eliminar el post. " + ex });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(new { isSuccess = false, message = ex.Message });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { isSuccess = false, message = ex.Message });
            }
        }


        [HttpGet]
        [Route("ObtenerPostsSeguidos/{usuarioId}")]
        public async Task<IActionResult> ObtenerPostsSeguidos(int usuarioId)
        {
            try
            {
                var jwtToken = Request.Cookies["jwt"];

                var principal = _utilidades.GetPrincipalFromToken(jwtToken!);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }

                var seguidores = await _context.Follows
                    .Where(f => f.FollowerId == usuarioId)
                    .Select(f => f.FolloweeId)
                    .ToListAsync();

                if (!seguidores.Any())
                {
                    return Ok(new { isSuccess = true, data = new List<object>() });
                }

                var postsSeguidos = await _context.Posts
                    .Where(p => seguidores.Contains(p.UsuarioId))
                    .Select(p => new
                    {
                        p.PostId,
                        p.Content,
                        p.Timestamp,
                        Usuario = _context.Users.Where(u => u.UsuarioId == p.UsuarioId).Select(u => u.Usuario).FirstOrDefault()
                    })
                    .ToListAsync();

                return Ok(new { isSuccess = true, data = postsSeguidos });
            }
            catch (Exception ex)
            {
                return BadRequest(new { isSuccess = false, message = "Error al obtener los posts seguidos.", error = ex.Message });
            }
        }

        // GET: api/Posts/Usuario/{usuarioId}
        [HttpGet("Usuario/{usuarioId}")]
        public async Task<ActionResult> GetUsuarioPosts(int usuarioId)
        {
            try
            {
                _logger.LogError("id usuario " + usuarioId);
                // Obtener el usuario autenticado desde el token JWT
                var jwtToken = Request.Cookies["jwt"];
                var principal = _utilidades.GetPrincipalFromToken(jwtToken!);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }



                // Obtener los posts del usuario por usuarioId
                var posts = await _context.Posts
                    .Where(p => p.UsuarioId == usuarioId)
                    .ToListAsync();

                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener posts del usuario {usuarioId}: {ex.Message}");
                return BadRequest("Error al obtener los posts del usuario.");
            }
        }



    }
}
