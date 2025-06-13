using EncuestaAPI.Models;
using EncuestaAPI.Models.DTOs;
using EncuestaAPI.Models.Validators;
using EncuestaAPI.Repositories;
using EncuestaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EncuestaAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        public UsuariosController(Repository<Usuario> repository, UsuarioValidator validador, JwtService service)
        {
            Repositoryy = repository;
            Validador = validador;
            Service = service;
        }
        public Repository<Usuario> Repositoryy { get; }
        public UsuarioValidator Validador { get; }
        public JwtService Service { get; }
        [HttpPost]
        public IActionResult Registrar(UsuarioDTO dto)
        {
            var usuario = new Usuario
            {
                Nombre = dto.Nombre ?? "",
                NumControl = dto.NumControl ?? "",
                Contrsena = dto.Contraseña ?? ""
            };
            var resultado = Validador.Validate(usuario);
            if (!resultado.IsValid)
            {
                return BadRequest(resultado.Errors);
            }
            Repositoryy.Insert(usuario);
            return Ok();
        }
        [Authorize]
        [HttpPost("cambiarpassword")]
        public IActionResult CambiarPassword()
        {
            return Ok();
        }
        [HttpPost("login")]
        public IActionResult Login(UsuarioDTO dto)
        {
            var token = Service.GenerateToken(dto);
            if (token == null)
            {
                return Unauthorized("Credenciales incorrectas");
            }
            return Ok(token);
        }
    }
}
