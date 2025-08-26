using SupportApi.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SupportApi.Services;
using SupportApi.Models;
using SupportApi.Data;

namespace SupportApi.Controllers
{
    /// <summary>Controlador de autenticación: registro y login de usuarios.</summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SupportDbContext _context;
        private readonly TokenService _tokenService;
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public AuthController(SupportDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordHasher = new PasswordHasher<Usuario>();
        }

        /// <summary>Registro de usuario nuevo.</summary>
        /// <param name="dto">Datos del usuario a registrar</param>
        /// <returns>Mensaje de éxito</returns>
        /// <response code="200">Usuario registrado correctamente</response>
        /// <response code="400">Datos inválidos</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] UsuarioCreateDto dto)
        {
            // Validar email duplicado
            var existe = _context.Usuarios.Any(u => u.CorreoElectronico.Trim().ToLower() == dto.CorreoElectronico.Trim().ToLower());
            if (existe)
            {
                return BadRequest(new { Message = "El correo electrónico ya está registrado." });
            }

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = dto.Nombre,
                CorreoElectronico = dto.CorreoElectronico,
                Rol = dto.Rol
            };

            usuario.HashContrasena = _passwordHasher.HashPassword(usuario, dto.Contrasena);

            try
            {
                _context.Usuarios.Add(usuario);
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return StatusCode(500, "No se pudo guardar el usuario en la base de datos.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar el usuario: {ex.Message}");
            }

            // Devolver el usuario creado para depuración
            return Ok(new {
                usuario.Id,
                usuario.Nombre,
                usuario.CorreoElectronico,
                usuario.Rol
            });
        }

        /// <summary>Login de usuario y generación de token JWT.</summary>
        /// <param name="dto">Credenciales de usuario</param>
        /// <returns>Token JWT</returns>
        /// <response code="200">Token generado correctamente</response>
        /// <response code="401">Credenciales inválidas</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] UsuarioLoginDto dto)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.CorreoElectronico == dto.CorreoElectronico);
            if (usuario == null)
                return Unauthorized(new { Message = "El usuario no existe o el correo es incorrecto. Por favor, regístrese primero." });

            var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.HashContrasena, dto.Contrasena);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(new { Message = "Contraseña incorrecta." });

            var token = _tokenService.GenerateToken(usuario);
            return Ok(new { Token = token });
        }
    }
}
