using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Back.Domain.Models;
using Back.Infrastructure.Data;
using Back.Presentacion.Custom;

namespace Back.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SocialNetworkDbContext _context;
        private readonly Utilidades _utilidades;

        public UsersController(SocialNetworkDbContext context, Utilidades utilidades)
        {
            _context = context;
            _utilidades = utilidades;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var jwtToken = Request.Cookies["jwt"];
                var principal = _utilidades.GetPrincipalFromToken(jwtToken!);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }

                var usuarios = await _context.Users.Select(u => new { u.UsuarioId, u.Usuario }).ToListAsync();
                return Ok(new { isSuccess = true, data = usuarios });
            }
            catch (Exception ex)
            {
                return BadRequest(new { isSuccess = false, message = ex.Message });
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}", Name = "GetUsuario")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                var jwtToken = Request.Cookies["jwt"];
                var principal = _utilidades.GetPrincipalFromToken(jwtToken!);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }


                var user = await _context.Users.FirstOrDefaultAsync(g => g.UsuarioId == id);

                if (user == null)
                {
                    return NotFound();
                }

                return user;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Users/ByUserName/andres
        [HttpGet("ByUserName/{usuario}")]
        public async Task<ActionResult<User>> GetUserByUserName(string usuario)
        {

            try
            {
                var jwtToken = Request.Cookies["jwt"];
                var principal = _utilidades.GetPrincipalFromToken(jwtToken);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }

                var user = await _context.Users.FirstOrDefaultAsync(g => g.Usuario == usuario);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user); // Devuelve todo el objeto de usuario encontrado
            }
            catch (Exception ex)
            {
                return BadRequest(new { isSuccess = false, message = ex.Message });
            }
        }



        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            try
            {
                var jwtToken = Request.Cookies["jwt"];
                var principal = _utilidades.GetPrincipalFromToken(jwtToken!);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }


                // Verificar si el nombre de usuario, el correo electrónico o el número ya existen en la base de datos
                bool usuarioExistente = await _context.Users.AnyAsync(u => u.Usuario == user.Usuario);
                bool emailExistente = await _context.Users.AnyAsync(u => u.Email == user.Email);
                bool numeroExistente = await _context.Users.AnyAsync(u => u.Numero == user.Numero);

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

                // Si no existen conflictos, agregar el nuevo usuario a la base de datos
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Devolver el resultado exitoso con CreatedAtRoute
                return CreatedAtRoute("GetUsuario", new { id = user.UsuarioId }, user);
            }
            catch (Exception ex)
            {
                // En caso de excepción, devolver el mensaje de error
                Console.WriteLine(ex.ToString());
                return BadRequest(new { isSuccess = false, message = "Error al registrar, intentelo mas tarde" });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchUser(int id, [FromBody] User updatedUser)
        {
            try
            {
                var jwtToken = Request.Cookies["jwt"];
                var principal = _utilidades.GetPrincipalFromToken(jwtToken!);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }

                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                // Actualizar sólo los campos que fueron proporcionados en el request
                if (updatedUser.Usuario != null)
                {
                    user.Usuario = updatedUser.Usuario;
                }

                if (updatedUser.Email != null)
                {
                    user.Email = updatedUser.Email;
                }

                if (updatedUser.Numero != null)
                {
                    user.Numero = updatedUser.Numero;
                }

                if (updatedUser.Clave != null)
                {
                    // Encriptar la nueva contraseña antes de actualizar
                    user.Clave = _utilidades.encriptarSha256(updatedUser.Clave);
                }

                await _context.SaveChangesAsync();

                return CreatedAtRoute("GetUsuario", new { id = user.UsuarioId }, user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(new { isSuccess = false, message = "Error al actualizar usuario, inténtelo más tarde" });
            }
        }





        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var jwtToken = Request.Cookies["jwt"];
                var principal = _utilidades.GetPrincipalFromToken(jwtToken!);

                if (principal == null)
                {
                    return Unauthorized(new { isSuccess = false, message = "Usuario no autorizado." });
                }

                var user = await _context.Users.FirstOrDefaultAsync(g => g.UsuarioId == id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    return Ok(id);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(new { isSuccess = false, message = "Error al borrar usuario, intentelo mas tarde" });
            }
        }

    }
}
